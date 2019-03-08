using System;
using System.Collections.Generic;

namespace WebCrawler.Models
{
    public partial class Page
    {
        public Page()
        {
            Image = new HashSet<Image>();
            LinkFromPageNavigation = new HashSet<Link>();
            LinkToPageNavigation = new HashSet<Link>();
            PageData = new HashSet<PageData>();
        }

        public int Id { get; set; }
        public int? SiteId { get; set; }
        public string PageTypeCode { get; set; }
        public string Url { get; set; }
        public string HtmlContent { get; set; }
        public int? HttpStatusCode { get; set; }
        public DateTime? AccessedTime { get; set; }

        public PageType PageTypeCodeNavigation { get; set; }
        public Site Site { get; set; }
        public ICollection<Image> Image { get; set; }
        public ICollection<Link> LinkFromPageNavigation { get; set; }
        public ICollection<Link> LinkToPageNavigation { get; set; }
        public ICollection<PageData> PageData { get; set; }
    }
}
