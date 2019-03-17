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
        public static TransformBlock<LoadedSite, IHtmlDocument> GetBlock(IServiceScopeFactory scopeFactory)
        {
            return new TransformBlock<LoadedSite, IHtmlDocument>(async document =>
            {
                Log.Information("Site parser");

                var scope = scopeFactory.CreateScope();
                var dbContext = (DbContext)scope.ServiceProvider.GetService(typeof(DbContext));
                var pages = dbContext.Page.ToList();
                await dbContext.Page.AddAsync(new Page()
                {
                    Url = document.Url,
                    AccessedTime = DateTime.Now,
                    HtmlContent = document.DocumentSource,
                    HttpStatusCode = document.ResponseCode,
                    PageTypeCode = "HTML",
                    SiteId = dbContext.Site.Where(s => s.Domain == document.Domain).FirstOrDefault()?.Id
                });
                await dbContext.SaveChangesAsync();
                scope.Dispose();

                var parser = new HtmlParser();
                var doc = await parser.ParseDocumentAsync(document.DocumentSource);

                return doc;
            });
        }
    }
}
