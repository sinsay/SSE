using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Indexer;
using Common;

namespace Searcher.Processor
{
    public interface ISearchProcessor
    {
        void Execute<T>(IContext context);
    }
}
