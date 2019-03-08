using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace WebCrawler.Crawling
{
    public class Frontier
    {
        public static BufferBlock<Uri> GetBlock()
        {
            return new BufferBlock<Uri>(new DataflowBlockOptions()
            {
                EnsureOrdered = true
            });
        }
    }
}
