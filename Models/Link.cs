using System;
using System.Collections.Generic;

namespace WebCrawler.Models
{
    public partial class Link
    {
        public int FromPage { get; set; }
        public int ToPage { get; set; }

        public Page FromPageNavigation { get; set; }
        public Page ToPageNavigation { get; set; }
    }
}
