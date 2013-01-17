using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indexer.IndexInterface
{
    /// <summary>
    /// interface 4 index directory
    /// </summary>
    public interface IIndexDirectory
    {
        string IndexPath { get; set; }

        bool Ram { get; set; }

        /// <summary>
        /// call this method before truly use it for search or indexing
        /// </summary>
        void Init();

        bool Exists();

        /// <summary>
        /// Close the directory
        /// </summary>
        void Close();
    }
}
