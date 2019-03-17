using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;
using static WebCrawler.Crawling.PageLoader;

namespace WebCrawler.Crawling
{
    public class PageParser
    {
        public static TransformBlock<Page, IHtmlDocument> GetBlock(IServiceScopeFactory scopeFactory)
        {
            return new TransformBlock<Page, IHtmlDocument>(async page =>
            {
                Log.Information("Site parser");

                var scope = scopeFactory.CreateScope();
                var dbContext = (DbContext)scope.ServiceProvider.GetService(typeof(DbContext));
                page.PageTypeCode = "HTML";
                page.SiteId = dbContext.Site.Where(s => s.Domain == page.Domain).FirstOrDefault()?.Id;
                dbContext.Page.Update(page);
                await dbContext.SaveChangesAsync();
                scope.Dispose();

                var parser = new HtmlParser();
                var doc = await parser.ParseDocumentAsync(page.HtmlContent);

                return doc;
            });
        }
    }
}
