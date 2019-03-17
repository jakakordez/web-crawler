using Louw.SitemapParser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobotsTxt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;

namespace WebCrawler.Crawling
{
    public class SiteLoader
    {
        public static TransformBlock<Uri, Uri> GetBlock(IServiceScopeFactory scopeFactory, BufferBlock<Uri> frontier)
        {
            return new TransformBlock<Uri, Uri>(async uri => {
                var domainRegex = new Regex(@"https?:\/\/(.+?)\/");
                var domain = domainRegex.Match(uri.ToString()).Groups[1].Value;

                var scope = scopeFactory.CreateScope();
                var dbContext = (Models.DbContext)scope.ServiceProvider.GetService(typeof(Models.DbContext));
                var site = dbContext.Site.Where(s => s.Domain == domain).FirstOrDefault();
                if(site == null)
                {
                    var client = new HttpClient();

                    var response = await client.GetAsync("http://"+domain+"/robots.txt");
                    string robotsContent = null, sitemapContent = null;
                    if (response.IsSuccessStatusCode)
                    {
                        robotsContent = await response.Content.ReadAsStringAsync();
                        var r = Robots.Load(robotsContent);

                        if (r.Sitemaps.Count > 0)
                        {
                            response = await client.GetAsync(r.Sitemaps[0].Url);
                            if (response.IsSuccessStatusCode)
                            {
                                sitemapContent = await response.Content.ReadAsStringAsync();

                                var sitemap = new SitemapParser().Parse(sitemapContent);

                                foreach (var item in sitemap.Items)
                                {
                                    frontier.Post(item.Location);
                                }
                            }
                        }
                    }

                    await dbContext.Site.AddAsync(new Site()
                    {
                        Domain = domain,
                        RobotsContent = robotsContent,
                        SitemapContent = sitemapContent
                    });
                    await dbContext.SaveChangesAsync();
                }
                scope.Dispose();

                return uri;
            });
        }
    }
}
