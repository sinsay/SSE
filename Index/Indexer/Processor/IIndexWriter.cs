using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Indexer.IndexInterface;

namespace Indexer.Processor
{
    /// <summary>
    /// interface for index processor
    /// </summary>
    public interface IIndexWriter
    {
        IIndexDirectory Directory { get; set; }

        /// <summary>
        /// index doc to the directory
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        bool AddDocument(Document doc);

        /// <summary>
        /// close the directory
        /// </summary>
        void Close();

        /// <summary>
        /// flush and commit the cache or the directory to the index directory
        /// </summary>
        void Commit();
    }
}