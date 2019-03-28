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
        public static TransformBlock<Page, Page> GetBlock()
        {
            return new TransformBlock<Page, Page>(async page =>
            {
                if (page == null) return null;

                Log.Information("Page parser {0}", page.Url);
                if (page.PageTypeCode == "HTML")
                {
                    var parser = new HtmlParser();
                    var doc = await parser.ParseDocumentAsync(page.HtmlContent);
                    page.document = doc;
                }
                return page;
            });
        }
    }
}
