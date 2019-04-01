using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace WebCrawler.Crawling
{
    public partial class Crawler
    {
        public static IPropagatorBlock<T[], T> CreateSelectManyBlock<T>()
        {
            // The source part of the propagator holds arrays of size windowSize
            // and propagates data out to any connected targets.
            var source = new BufferBlock<T>();

            // The target part receives data and adds them to the queue.
            var target = new ActionBlock<T[]>(items =>
            {
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        source.Post(item);
                    }
                }
            });

            // When the target is set to the completed state, propagate out any
            // remaining data and set the source to the completed state.
            target.Completion.ContinueWith(delegate
            {
                source.Complete();
            });

            // Return a IPropagatorBlock<T, T[]> object that encapsulates the 
            // target and source blocks.
            return DataflowBlock.Encapsulate(target, source);
        }
    }
}
