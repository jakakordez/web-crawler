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
        public struct LoadedSite
        {
            public string DocumentSource;
            public int ResponseCode;
            public string Url;
            public string Domain => 
                new Regex(@"https?:\/\/(.+?)\/").Match(Url.ToString()).Groups[1].Value;
            
        }

        public static TransformBlock<Page, Page> GetBlock()
        {
            return new TransformBlock<Page, Page>(page =>
            {
                Log.Information("Site loader {0}", page.Url);
                ChromeDriver browser = null;
                try
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
                    page.HttpStatusCode = int.Parse(responseCode);
                }
                catch(Exception e)
                {
                    Log.Error(e, "Chrome error");
                }
                finally
                {
                    if(browser != null) BrowserPool.Return(browser);
                }

                return page;
            });
        }
    }
}
