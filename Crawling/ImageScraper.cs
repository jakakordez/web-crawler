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
        public static TransformBlock<Page, Image[]> GetBlock(IServiceScopeFactory scopeFactory)
        {
            return new TransformBlock<Page, Image[]>(async page =>
            {
                var imgs = page.document.QuerySelectorAll("img");
                // TODO: relative urls
                Log.Information("Image scraper found {0} images", imgs.Length);
                var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<Models.DbContext>();
                var images = new List<Image>();
                foreach (var img in imgs)
                {
                    var src = img.GetAttribute("src");
                    Image image = new Image();
                    image.PageId = page.Id;
                    image.Filename = src;
                    await dbContext.Image.AddAsync(image);
                    images.Add(image);
                }
                await dbContext.SaveChangesAsync();
                scope.Dispose();
                return images.ToArray();
            });
        }
    }
}
