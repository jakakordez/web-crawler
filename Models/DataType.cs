using System;
using System.Collections.Generic;

namespace WebCrawler.Models
{
    public partial class DataType
    {
        public DataType()
        {
            PageData = new HashSet<PageData>();
        }

        public string Code { get; set; }

        public ICollection<PageData> PageData { get; set; }
    }
}
