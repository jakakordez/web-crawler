using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;
using WebCrawler.Crawling;

namespace WebCrawler.Controllers
{
    [Route("api/status")]
    [ApiController]
    public class CrawlerController:ControllerBase
    {

        struct CrawlerStatus
        {
            public int frontier;
            public int pageLoader, siteLoader, pageParser;
            public int imageScraper;
            public int linkScraper;
            public int imageLoader;
        }
        
        [HttpGet]
        public ActionResult Get()
        {
            return new JsonResult(new CrawlerStatus()
            {
                frontier = Crawler.frontier.Count,
                pageLoader = Crawler.pageLoader.InputCount,
                siteLoader = Crawler.siteLoader.InputCount,
                pageParser = Crawler.pageParser.InputCount,
                imageScraper = Crawler.imageScraper.InputCount,
                linkScraper = Crawler.linkScraper.InputCount,
                imageLoader = Crawler.imageLoader.InputCount
            });
        }
    }
}
