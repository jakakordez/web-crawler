using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using Serilog;
using WebCrawler.Models;
using Page = WebCrawler.Models.Page;

namespace WebCrawler.Crawling
{
    public class PageLoader
    {
        private static String GetPageType(String url)
        {
            if (url.EndsWith(".pdf"))
                return "PDF";
            if (url.EndsWith(".doc"))
                return "DOC";
            if (url.EndsWith(".docx"))
                return "DOCX";
            if (url.EndsWith(".ppt"))
                return "PPT";
            if (url.EndsWith(".pptx"))
                return "PPTX";
            return null;
        }

        public static TransformBlock<Page, Page> GetBlock(IServiceScopeFactory scopeFactory)
        {
            return new TransformBlock<Page, Page>(async page =>
            {
                if (page == null) return null;

                Log.Information("Page loader {0}", page.Url);
                ChromeDriver browser = null;

                var scope = scopeFactory.CreateScope();
                var dbContext = (DbContext)scope.ServiceProvider.GetService(typeof(DbContext));
                try
                {
                    String pageType = GetPageType(page.Url);
                    if (pageType != null)
                    {
                        Log.Information("Downloading pdf {0}", page.Url);

                        HttpClient client = new HttpClient();
                        var res = await client.GetAsync(page.Url);
                        var pageData = new PageData
                        {
                            PageId = page.Id,
                            Data = await res.Content.ReadAsByteArrayAsync(),
                            DataTypeCode = pageType
                        };
                        var ContentType = res.Content.Headers.ContentType.MediaType;

                        await dbContext.PageData.AddAsync(pageData);

                        page.PageTypeCode = "BINARY";
                        page.HttpStatusCode = (int)res.StatusCode;
                    }
                    else
                    {
                        browser = BrowserPool.Get();
                        browser.Navigate().GoToUrl(page.Url);
                        var logs = browser.Manage().Logs.GetLog("performance");
                        var responseCode = logs
                            .Select(log => JObject.Parse(log.Message)["message"])
                            .Where(o => o["method"]?.ToString() == "Network.responseReceived")
                            .Select(o => o["params"]["response"]["status"])
                            .First().ToString();
                        page.AccessedTime = DateTime.Now;
                        page.HtmlContent = browser.PageSource;
                        page.PageTypeCode = "HTML";
                        page.HttpStatusCode = int.Parse(responseCode);
                    }
                }
                catch(Exception e)
                {
                    Log.Error(e, "Chrome error");
                }
                finally
                {
                    if(browser != null) BrowserPool.Return(browser);
                    dbContext.Page.Update(page);
                    await dbContext.SaveChangesAsync();
                    scope.Dispose();
                }

                return page;
            }, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = 4
            });
        }
    }
}
