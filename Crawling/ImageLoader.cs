using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;

namespace WebCrawler.Crawling
{
    public class ImageLoader
    {
        public static ActionBlock<Image> GetBlock(IServiceScopeFactory scopeFactory)
        {
            return new ActionBlock<Image>(async i =>
            {
                if (i == null) return;

                try
                { 
                    HttpClient client = new HttpClient();
                    var res = await client.GetAsync(i.Filename);
                    i.Data = await res.Content.ReadAsByteArrayAsync();
                    i.AccessedTime = DateTime.Now;
                    i.ContentType = res.Content.Headers.ContentType.MediaType;

                    var scope = scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetService<Models.DbContext>();
                    dbContext.Image.Update(i);
                    await dbContext.SaveChangesAsync();
                    scope.Dispose();
                }
                catch (Exception e){
                    Log.Error(e, "Image loader exception");
                }
            });
        }
    }
}
