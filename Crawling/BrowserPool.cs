using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Concurrent;

namespace WebCrawler.Crawling
{
    public class BrowserPool
    { 
        private static ConcurrentQueue<ChromeDriver> browserPool 
            = new ConcurrentQueue<ChromeDriver>();

        public static ChromeDriver Get()
        {
            /*var service = FirefoxDriverService.CreateDefaultService(Environment.CurrentDirectory+ @"\bin\Debug\netcoreapp2.1", "geckodriver.exe");
                service.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                var options = new FirefoxOptions();
                options.AddArgument("--headless");

                var browser = new FirefoxDriver(service, options);
                
                browser.Navigate().GoToUrl(url);
                return browser.PageSource;*/

            var r = browserPool.TryDequeue(out ChromeDriver browser);
            if (r) return browser;

            var service = ChromeDriverService.CreateDefaultService(Environment.CurrentDirectory + @"\bin\Debug\netcoreapp2.2", "chromedriver.exe");
            var chromeOptions = new ChromeOptions();

            // Disable images
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            chromeOptions.AddArguments("headless");
            chromeOptions.AddArgument("--user-agent=" + Crawler.CrawlerName);
            chromeOptions.SetLoggingPreference("performance", LogLevel.All);

            return new ChromeDriver(service, chromeOptions);
        }

        public static void Return(ChromeDriver driver)
        {
            browserPool.Enqueue(driver);
        }
    }
}
