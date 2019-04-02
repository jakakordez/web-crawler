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
        public static readonly object lockObj = new object();

        public static BufferBlock<Page> frontier;
        public static TransformBlock<Page, Page> siteLoader, pageLoader, checkIfDuplicate, pageParser;
        public static TransformBlock<Page, Image[]> imageScraper;
        public static ActionBlock<Page> linkScraper;
        public static ActionBlock<Image> imageLoader;

        public static async Task StartCrawler(IServiceScopeFactory scopeFactory)
        {
            frontier = Frontier.GetBlock();
            siteLoader = SiteLoader.GetBlock(scopeFactory, frontier);
            pageLoader = PageLoader.GetBlock(scopeFactory);
            checkIfDuplicate = CheckIfDuplicate.GetBlock(scopeFactory);
            pageParser = PageParser.GetBlock();
            linkScraper = LinkScraper.GetBlock(scopeFactory, frontier);
            imageScraper = ImageScraper.GetBlock(scopeFactory);
            imageLoader = ImageLoader.GetBlock(scopeFactory);
            var domBroadcast = new BroadcastBlock<Page>(d => d,
                new DataflowBlockOptions());
            var imageSelect = CreateSelectManyBlock<Image>();

            frontier.LinkTo(siteLoader, new DataflowLinkOptions());
            siteLoader.LinkTo(pageLoader, new DataflowLinkOptions());
            pageLoader.LinkTo(checkIfDuplicate, new DataflowLinkOptions());
            checkIfDuplicate.LinkTo(pageParser, new DataflowLinkOptions());
            pageParser.LinkTo(domBroadcast, new DataflowLinkOptions());
            domBroadcast.LinkTo(linkScraper, new DataflowLinkOptions());
            domBroadcast.LinkTo(imageScraper, new DataflowLinkOptions());
            imageScraper.LinkTo(imageSelect, new DataflowLinkOptions());
            imageSelect.LinkTo(imageLoader, new DataflowLinkOptions());

            var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<DbContext>();
            await PostPage(new Uri("http://evem.gov.si"), dbContext, frontier, null);
            await PostPage(new Uri("http://e-uprava.gov.si"), dbContext, frontier, null);
            await PostPage(new Uri("http://podatki.gov.si"), dbContext, frontier, null);
            await PostPage(new Uri("http://e-prostor.gov.si"), dbContext, frontier, null);
            scope.Dispose();
        }

        public static async Task<Page> PostPage(Uri uri, DbContext dbContext, BufferBlock<Page> frontier, int? previous_page_id)
        {
            uri = new Uri(uri.ToString().Replace("www.", "").Replace(".html", "").ToLower().Split('?')[0].Split('#')[0]);

            var govsiRegex = new Regex(@"https?:\/\/[^\/]+gov\.si");
            if (!govsiRegex.IsMatch(uri.ToString())) return null;

            Page page;
            lock (Crawler.lockObj)
            {
               page = dbContext.Page.Where(d => d.Url == uri.ToString()).FirstOrDefault();
            }

            if(page == null)
            {
                page = new Page()
                {
                    Url = uri.ToString(),
                    PageTypeCode = "FRONTIER"
                };
                try
                {
                    lock (lockObj)
                    {
                        dbContext.Page.Add(page);
                        dbContext.SaveChanges();
                    }
                    frontier.Post(page);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Post page error");
                    lock (Crawler.lockObj)
                    {
                        page = dbContext.Page.Where(d => d.Url == uri.ToString()).FirstOrDefault();
                    }
                }
            }
            
            try
            {
                if (previous_page_id != null)
                {
                    lock (Crawler.lockObj)
                    {
                        var link = dbContext.Link.Where(l => l.FromPage == previous_page_id && l.ToPage == page.Id).FirstOrDefault();
                        if (link == null)
                        {
                            lock (lockObj)
                            {
                                dbContext.Link.Add(new Link
                                {
                                    FromPage = (int)previous_page_id,
                                    ToPage = page.Id
                                });
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Post page link error");
            }

            return page;
        }
    }
}
