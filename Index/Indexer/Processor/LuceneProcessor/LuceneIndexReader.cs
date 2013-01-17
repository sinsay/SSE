using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Indexer;
using Indexer.IndexInterface;

namespace Indexer.Processor.LuceneProcessor
{
    public class LuceneIndexReader : IIndexReader, IDisposable
    {
        public IIndexDirectory Directory { get; set; }

        public Lucene.Net.Index.IndexReader IndexReader { get; set; }

        public LuceneIndexReader(IIndexDirectory dir)
        {
            this.Directory = dir;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
