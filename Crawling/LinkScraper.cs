using AngleSharp;
using Knyaz.Optimus.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;
using static WebCrawler.Crawling.PageLoader;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace WebCrawler.Crawling
{
    public class LinkScraper
    {
        public static ActionBlock<IHtmlDocument> GetBlock(IServiceScopeFactory scopeFactory)
        {
            return new ActionBlock<IHtmlDocument>(async document =>
            {
                var links = document.QuerySelectorAll("a");
                Log.Information("Link scraper found {0} links", links.Length);
            });
        }
    }
}
