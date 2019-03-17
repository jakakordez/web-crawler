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

        public static TransformBlock<Uri, LoadedSite> GetBlock()
        {
            return new TransformBlock<Uri, LoadedSite>(url =>
            {
                Log.Information("Site loader {0}", url);

                var browser = BrowserPool.Get();  
                browser.Navigate().GoToUrl(url);
                var logs = browser.Manage().Logs.GetLog("performance");
                var responseCode = logs
                    .Select(log => JObject.Parse(log.Message)["message"])
                    .Where(o => o["method"]?.ToString() == "Network.responseReceived")
                    .Select(o => o["params"]["response"]["status"])
                    .First().ToString();

                var loadedSite = new LoadedSite()
                {
                    DocumentSource = browser.PageSource,
                    ResponseCode = int.Parse(responseCode),
                    Url = url.ToString()
                };

                BrowserPool.Return(browser);
                return loadedSite;
            });
        }
    }
}
