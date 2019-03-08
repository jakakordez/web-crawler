using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;

namespace WebCrawler.Crawling
{
    public static class Crawler
    {
        public static IWebHost RunCrawler(this IWebHost webHost)
        {
            var dbContext = (DbContext)webHost.Services.GetService(typeof(DbContext));

            var siteLoader = SiteLoader.GetBlock();
            var frontier = Frontier.GetBlock();
            var siteParser = SiteParser.GetBlock(dbContext);

            frontier.LinkTo(siteLoader, new DataflowLinkOptions());
            siteLoader.LinkTo(siteParser, new DataflowLinkOptions());

            frontier.Post(new Uri("http://evem.gov.si"));
            frontier.Post(new Uri("http://e-uprava.gov.si"));
            frontier.Post(new Uri("http://podatki.gov.si"));
            frontier.Post(new Uri("http://e-prostor.gov.si"));

            return webHost;
        }
    }
}
