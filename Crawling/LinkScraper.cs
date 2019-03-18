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

namespace WebCrawler.Crawling
{
    public class LinkScraper
    {
        public static ActionBlock<Page> GetBlock(IServiceScopeFactory scopeFactory, BufferBlock<Page> frontier)
        {
            return new ActionBlock<Page>(async page =>
            {
                var redirectRegex = new Regex("(?:document\\.location|location\\.href)\\s?=\\s?(?:'|\")([^'\"])+(?:'|\")");
                var redirects = redirectRegex.Matches(page.HtmlContent).Select(m => m.Groups[1].Value);

                var links = page.document.QuerySelectorAll("a").Select(l => l.GetAttribute("href"));
                Log.Information("Link scraper found {0} links and {1} redirects", links.Count(), redirects.Count());

                var list = links.ToList();
                list.AddRange(redirects);

                var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<DbContext>();
                foreach (var url in list)
                {
                    try
                    {
                        var httpRegex = new Regex(@"https?:\/\/");
                        if (!httpRegex.IsMatch(url))
                        {
                            await Crawler.PostPage(new Uri(page.Url+url), dbContext, frontier);
                        }
                        else await Crawler.PostPage(new Uri(url), dbContext, frontier);
                    }
                    catch { }
                }
                scope.Dispose();
            });
        }
    }
}
