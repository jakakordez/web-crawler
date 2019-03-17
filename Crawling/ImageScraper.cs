using AngleSharp.Html.Dom;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static WebCrawler.Crawling.PageLoader;

namespace WebCrawler.Crawling
{
    public class ImageScraper
    {
        public static ActionBlock<IHtmlDocument> GetBlock(IServiceScopeFactory scopeFactory)
        {
            return new ActionBlock<IHtmlDocument>(async document =>
            {
                var imgs = document.QuerySelectorAll("img");
                Log.Information("Image scraper found {0} images", imgs.Length);
                
            });
        }
    }
}
