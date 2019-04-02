using AngleSharp;
using Knyaz.Optimus.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;
using static WebCrawler.Crawling.PageLoader;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using RobotsTxt;

namespace WebCrawler.Crawling
{
    public class LinkScraper
    {
        public static ActionBlock<Page> GetBlock(IServiceScopeFactory scopeFactory, BufferBlock<Page> frontier)
        {
            return new ActionBlock<Page>(async page =>
            {
                if (page == null) return;

                Log.Information("Link scraper {0}", page.Url);
                // If page is not html
                if (page.document == null)
                    return;

                var redirectRegex = new Regex("(?:document\\.location|location\\.href)\\s?=\\s?(?:'|\")([^'\"])+(?:'|\")");
                var redirects = redirectRegex.Matches(page.HtmlContent).Select(m => m.Groups[1].Value);

                var links = page.document.QuerySelectorAll("a").Select(l => l.GetAttribute("href"));
                Log.Information("Link scraper found {0} links and {1} redirects", links.Count(), redirects.Count());

                var list = links.ToList();
                list.AddRange(redirects);

                lock (Crawler.lockObj) { 
                    var scope = scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetService<DbContext>();
                    var site = dbContext.Site.Where(d => d.Id == page.SiteId).FirstOrDefault();
                    // Log.Information("Site: {0} for page: {1}", site.Domain, page.Url);
                    var r = Robots.Load(site.RobotsContent);

                    foreach (var url in list)
                    {
                        if (url == null) continue;
                        try
                        {
                            var httpRegex = new Regex(@"https?:\/\/");
                            var absoluteUrl = url;
                            if (!httpRegex.IsMatch(url))
                            {
                                absoluteUrl = page.Url + url;
                            }
                            if (r.IsPathAllowed(Crawler.CrawlerName, absoluteUrl))
                            {
                                Crawler.PostPage(new Uri(absoluteUrl), dbContext, frontier, page.Id).Wait();
                            }
                            else
                            {
                                Log.Information("Url: {0} is not allowed", absoluteUrl);
                            }
                        }
                        catch { }
                    }
                    scope.Dispose();
                }

            });
        }
    }
}
