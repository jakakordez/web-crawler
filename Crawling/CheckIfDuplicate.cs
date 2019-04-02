using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;

namespace WebCrawler.Crawling
{
    public class CheckIfDuplicate
    {
        public static TransformBlock<Page, Page> GetBlock(IServiceScopeFactory scopeFactory)
        {
            return new TransformBlock<Page, Page>(async page =>
            {
                if (page == null) return null;
                if (page.HtmlContent == null) return page;

                Log.Information("Check if duplicate {0}", page.Url);

                var scope = scopeFactory.CreateScope();
                var dbContext = (DbContext)scope.ServiceProvider.GetService(typeof(DbContext));
                Page duplicate;
                try
                {
                    lock (Crawler.lockObj)
                    {
                        duplicate = dbContext.Page.Where(p => p.HtmlContent == page.HtmlContent && p.Id != page.Id && string.Compare(p.PageTypeCode, "HTML") == 0).FirstOrDefault();
                    }
                    if (duplicate != null)
                    {
                        Log.Information("Duplicate found! 1. URL: {0}, 2. URL: {1}", duplicate.Url, page.Url);
                        page.HtmlContent = null;
                        page.PageTypeCode = "DUPLICATE";

                        await dbContext.Link.AddAsync(new Link
                        {
                            FromPage = page.Id,
                            ToPage = duplicate.Id
                        });
                    }
                    duplicate = null;
                    lock (Crawler.lockObj)
                    {
                        dbContext.Page.Update(page);
                        dbContext.SaveChanges();
                    }
                    scope.Dispose();

                    return duplicate == null ? page : null;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Check if duplicate error");
                }

                return page;
            });
        }
    }
}
