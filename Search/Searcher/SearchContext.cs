using Indexer.IndexInterface;
using Query.Expression.Node;
using Query;
using Common;

namespace Searcher
{
    /// <summary>
    /// search context
    /// </summary>
    public class SearchContext<T>: IContext
    {
        public QueryContext QueryContext { get; set; }

        /// <summary>
        /// pageindex
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// pagesize
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// search result 
        /// </summary>
        public SearchResult<T> Result { get; set; }

        /// <summary>
        /// search index pathes
        /// </summary>
        public IIndexDirectory[] Pathes { get; set; }
    }
}
