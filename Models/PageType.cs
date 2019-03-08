using System;
using System.Collections.Generic;

namespace WebCrawler.Models
{
    public partial class PageType
    {
        public PageType()
        {
            Page = new HashSet<Page>();
        }

        public string Code { get; set; }

        public ICollection<Page> Page { get; set; }
    }
}
