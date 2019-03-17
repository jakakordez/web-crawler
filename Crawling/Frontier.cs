using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Models;

namespace WebCrawler.Crawling
{
    public class Frontier
    {
        public static BufferBlock<Page> GetBlock()
        {
            return new BufferBlock<Page>(new DataflowBlockOptions()
            {
                EnsureOrdered = true
            });
        }
    }
}
