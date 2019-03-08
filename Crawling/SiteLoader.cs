using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace WebCrawler.Crawling
{
    public class SiteLoader
    {
        public struct LoadedSite
        {
            public string DocumentSource;
            public int ResponseCode;
        }

        public static TransformBlock<Uri, LoadedSite> GetBlock()
        {
            return new TransformBlock<Uri, LoadedSite>(url =>
            {
                /*var service = FirefoxDriverService.CreateDefaultService(Environment.CurrentDirectory+ @"\bin\Debug\netcoreapp2.1", "geckodriver.exe");
                service.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                var options = new FirefoxOptions();
                options.AddArgument("--headless");

                var browser = new FirefoxDriver(service, options);
                
                browser.Navigate().GoToUrl(url);
                return browser.PageSource;*/
                
                var service = ChromeDriverService.CreateDefaultService(Environment.CurrentDirectory+ @"\bin\Debug\netcoreapp2.1", "chromedriver.exe");
                var chromeOptions = new ChromeOptions();

                chromeOptions.AddArguments("headless");
                chromeOptions.SetLoggingPreference("performance", LogLevel.All);
                using (var browser = new ChromeDriver(service, chromeOptions))
                {
                    
                    browser.Navigate().GoToUrl(url);
                    var logs = browser.Manage().Logs.GetLog("performance");
                    var responseCode = logs
                        .Select(log => JObject.Parse(log.Message)["message"])
                        .Where(o => o["method"]?.ToString() == "Network.responseReceived")
                        .Select(o => o["params"]["response"]["status"])
                        .First();
                    return new LoadedSite()
                    {
                        DocumentSource = browser.PageSource,
                        ResponseCode = responseCode
                    };
                }
            });
        }
    }
}
