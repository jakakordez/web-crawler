using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;

namespace WebCrawler.Crawling
{
    public static partial class Crawler
    {
        public static readonly String CrawlerName = "WIER_agent";

        public static async Task StartCrawler(IServiceScopeFactory scopeFactory)
        {
            var frontier = Frontier.GetBlock();
            var siteLoader = SiteLoader.GetBlock(scopeFactory, frontier);
            var pageLoader = PageLoader.GetBlock();
            var pageParser = PageParser.GetBlock(scopeFactory);
            var linkScraper = LinkScraper.GetBlock(scopeFactory, frontier);
            var imageScraper = ImageScraper.GetBlock(scopeFactory);
            var imageLoader = ImageLoader.GetBlock(scopeFactory);
            var domBroadcast = new BroadcastBlock<Page>(d => d,
                new DataflowBlockOptions());
            var imageSelect = CreateSelectManyBlock<Image>();

            frontier.LinkTo(siteLoader, new DataflowLinkOptions());
            siteLoader.LinkTo(pageLoader, new DataflowLinkOptions());
            pageLoader.LinkTo(pageParser, new DataflowLinkOptions());
            pageParser.LinkTo(domBroadcast, new DataflowLinkOptions());
            domBroadcast.LinkTo(linkScraper, new DataflowLinkOptions());
            domBroadcast.LinkTo(imageScraper, new DataflowLinkOptions());
            imageScraper.LinkTo(imageSelect, new DataflowLinkOptions());
            imageSelect.LinkTo(imageLoader, new DataflowLinkOptions());

            var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<DbContext>();
            await PostPage(new Uri("http://evem.gov.si"), dbContext, frontier);
            await PostPage(new Uri("http://e-uprava.gov.si"), dbContext, frontier);
            await PostPage(new Uri("http://podatki.gov.si"), dbContext, frontier);
            await PostPage(new Uri("http://e-prostor.gov.si"), dbContext, frontier);
            scope.Dispose();
        }

        public static async Task<Page> PostPage(Uri uri, DbContext dbContext, BufferBlock<Page> frontier)
        {
            uri = new Uri(uri.ToString().Replace("www.", "").ToLower().Split('?')[0]);

            var govsiRegex = new Regex(@"https?:\/\/[^\/]+gov\.si");
            if (!govsiRegex.IsMatch(uri.ToString())) return null;

            var page = dbContext.Page.Where(d => d.Url == uri.ToString()).FirstOrDefault();

            if(page == null)
            {
                page = new Page()
                {
                    Url = uri.ToString()
                };
                try
                {
                    await dbContext.Page.AddAsync(page);
                    await dbContext.SaveChangesAsync();
                    frontier.Post(page);
                }
                catch
                {
                    page = dbContext.Page.Where(d => d.Url == uri.ToString()).FirstOrDefault();
                }
            }

            return page;
        }
    }
}
