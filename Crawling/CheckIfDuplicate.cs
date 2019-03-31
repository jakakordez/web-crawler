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
                try
                {
                    var duplicate = dbContext.Page.Where(p => p.HtmlContent == page.HtmlContent && p.Id != page.Id).FirstOrDefault();
                    if (duplicate == null)
                    {
                        // dbContext.Page.Update(page);
                        // await dbContext.SaveChangesAsync();
                        // scope.Dispose();
                        return page;
                    }

                    Log.Information("Duplicate found! 1. URL: {0}, 2. URL: {1}", duplicate.Url, page.Url);
                    page.HtmlContent = null;
                    page.PageTypeCode = "DUPLICATE";

                    dbContext.Page.Update(page);
                    await dbContext.Link.AddAsync(new Link
                    {
                        FromPage = duplicate.Id,
                        ToPage = page.Id
                    });
                    await dbContext.SaveChangesAsync();
                    scope.Dispose();

                    return null;
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
