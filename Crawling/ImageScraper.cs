using AngleSharp.Html.Dom;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;
using static WebCrawler.Crawling.PageLoader;

namespace WebCrawler.Crawling
{
    public class ImageScraper
    {
        public static ActionBlock<Page> GetBlock(IServiceScopeFactory scopeFactory)
        {
            return new ActionBlock<Page>(async page =>
            {
                var imgs = page.document.QuerySelectorAll("img");
                // TODO: relative urls
                Log.Information("Image scraper found {0} images", imgs.Length);
                
            });
        }
    }
}
