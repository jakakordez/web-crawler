using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
            /*var dbContext = (DbContext)webHost.Services.GetService(typeof(DbContext));

            var siteLoader = SiteLoader.GetBlock();
            var frontier = Frontier.GetBlock();
            var siteParser = SiteParser.GetBlock(dbContext);
            var linkScraper = LinkScraper.GetBlock(dbContext);
            var imageScraper = ImageScraper.GetBlock(dbContext);
            var domBroadcast = new BroadcastBlock<IHtmlDocument>(d => d, 
                new DataflowBlockOptions());

            frontier.LinkTo(siteLoader, new DataflowLinkOptions());
            siteLoader.LinkTo(siteParser, new DataflowLinkOptions());
            siteParser.LinkTo(domBroadcast, new DataflowLinkOptions());
            domBroadcast.LinkTo(linkScraper, new DataflowLinkOptions());
            domBroadcast.LinkTo(imageScraper, new DataflowLinkOptions());

            frontier.Post(new Uri("http://evem.gov.si"));
            frontier.Post(new Uri("http://e-uprava.gov.si"));
            frontier.Post(new Uri("http://podatki.gov.si"));
            frontier.Post(new Uri("http://e-prostor.gov.si"));
            */
            return webHost;
        }

        public static void StartCrawler(IServiceScopeFactory scopeFactory)
        {
            var frontier = Frontier.GetBlock();
            var siteLoader = SiteLoader.GetBlock(scopeFactory, frontier);
            var pageLoader = PageLoader.GetBlock();
            var pageParser = PageParser.GetBlock(scopeFactory);
            var linkScraper = LinkScraper.GetBlock(scopeFactory);
            var imageScraper = ImageScraper.GetBlock(scopeFactory);
            var domBroadcast = new BroadcastBlock<IHtmlDocument>(d => d,
                new DataflowBlockOptions());

            frontier.LinkTo(siteLoader, new DataflowLinkOptions());
            siteLoader.LinkTo(pageLoader, new DataflowLinkOptions());
            pageLoader.LinkTo(pageParser, new DataflowLinkOptions());
            pageParser.LinkTo(domBroadcast, new DataflowLinkOptions());
            domBroadcast.LinkTo(linkScraper, new DataflowLinkOptions());
            domBroadcast.LinkTo(imageScraper, new DataflowLinkOptions());

            frontier.Post(new Uri("http://evem.gov.si"));
            frontier.Post(new Uri("http://e-uprava.gov.si"));
            frontier.Post(new Uri("http://podatki.gov.si"));
            frontier.Post(new Uri("http://e-prostor.gov.si"));
        }
    }
}
