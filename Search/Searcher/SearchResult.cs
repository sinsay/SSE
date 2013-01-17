using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Searcher
{
    public class SearchResult<T>
    {
        /// <summary>
        /// count of current query
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// search document's , it will fill all the store field
        /// </summary>
        public T[] Documents { get; set; }

        /// <summary>
        /// time of search span
        /// </summary>
        public long TotalMillseconds { get; set; }
    }
}
