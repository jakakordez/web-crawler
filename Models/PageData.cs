using System;
using System.Collections.Generic;

namespace WebCrawler.Models
{
    public partial class PageData
    {
        public int Id { get; set; }
        public int? PageId { get; set; }
        public string DataTypeCode { get; set; }
        public byte[] Data { get; set; }

        public DataType DataTypeCodeNavigation { get; set; }
        public Page Page { get; set; }
    }
}
