﻿using System;
using System.Collections.Generic;

namespace WebCrawler.Models
{
    public partial class Image
    {
        public int Id { get; set; }
        public int? PageId { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
        public DateTime? AccessedTime { get; set; }

        public Page Page { get; set; }
    }
}
