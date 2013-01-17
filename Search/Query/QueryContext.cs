using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Query.Expression.Node;

namespace Query
{
    public class QueryContext: IContext
    {
        /// <summary>
        /// the query now processing
        /// </summary>
        internal QueryNode CurrentQuery { get; set; }

        /// <summary>
        /// all the queries
        /// </summary>
        public QueryNode[] Querys { get; set; }

        /// <summary>
        /// final query result
        /// </summary>
        public object Query { get; set; }

        public object Filter { get; set; }

        public object Sort { get; set; }
    }
}
