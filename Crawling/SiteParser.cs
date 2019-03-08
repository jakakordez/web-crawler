using Knyaz.Optimus.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;
using static WebCrawler.Crawling.SiteLoader;

namespace WebCrawler.Crawling
{
    public class SiteParser
    {
        public static ActionBlock<LoadedSite> GetBlock(DbContext dbContext)
        {
            return new ActionBlock<LoadedSite>(document =>
            {
                dbContext.Page.Add(new Page()
                {
                    Url = "url",
                    AccessedTime = DateTime.Now,
                    HtmlContent = document.DocumentSource,
                    HttpStatusCode = document.ResponseCode,
                    PageTypeCode = "HTML"
                });
            });
        }
    }
}
