using System;
using System.Collections.Generic;

namespace WebCrawler.Models
{
    public partial class Site
    {
        public Site()
        {
            Page = new HashSet<Page>();
        }

        public int Id { get; set; }
        public string Domain { get; set; }
        public string RobotsContent { get; set; }
        public string SitemapContent { get; set; }

        public ICollection<Page> Page { get; set; }
    }
}
