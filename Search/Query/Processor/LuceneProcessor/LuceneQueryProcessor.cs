using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using LuceneExtension.QueryExtension;
using Query.Expression.Node;
using Lucene.Net.Search;

namespace Query.Processor.LuceneProcessor
{
    /// <summary>
    /// query processor for lucene version
    /// </summary>
    public class LuceneQueryProcessor: IQueryProcessor
    {
        /// <summary>
        /// execute to process the query with query context, and save the query at context.Query
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IContext context)
        {
            var queryContext = context as QueryContext;

            if (queryContext == null || queryContext.Querys == null)
                return;

            var query = new BooleanQuery();

            foreach (var q in queryContext.Querys)  // process all the queries
            {
                if (q == null)
                    continue;

                queryContext.CurrentQuery = q;
                Lucene.Net.Search.Query curQuery = null;

                // recursive process the sub query
                if (queryContext.CurrentQuery.Type == QueryType.Operation)
                {
                    var subContext = new QueryContext
                    {
                        Querys = q.RelationQuerys,
                    };

                    this.Execute(subContext);
                    curQuery = subContext.Query as Lucene.Net.Search.Query;
                }
                else  // process cur query
                {
                    // get the query attr for build opposite query
                    var attr = UtilityLib.Reflection.Attribute.GetAttributes<Common.Attributes.BaseAttribute>((q as QueryNode).FieldValue).FirstOrDefault();
                    if (attr == null)
                    {
                        throw new NotImplementedException("field value has no attr for query");
                    }

                    attr.Execute(queryContext);

                    if (queryContext.CurrentQuery.Type == QueryType.QueryField) // 其他的 Query Type 会将检索表达式放到对应的上下文字段
                    {
                        curQuery = queryContext.CurrentQuery.Query as Lucene.Net.Search.Query;
                    }
                }

                // if curQuery if not null, add it to query with logic
                if (curQuery != null)
                {
                    query = this.QueryOperator(
                            queryContext.CurrentQuery.Logic,
                            curQuery,
                            query);
                }
            }

            // save the final query
            queryContext.Query = query;
        }

        private Lucene.Net.Search.BooleanQuery QueryOperator(QueryLogic logic, Lucene.Net.Search.Query curQuery, Lucene.Net.Search.BooleanQuery targetQuery)
        {
            switch (logic)
            {
                case QueryLogic.MUST:
                    targetQuery = targetQuery.Must(curQuery);
                    break;
                case QueryLogic.SHOULD:
                    targetQuery = targetQuery.Should(curQuery);
                    break;
                case QueryLogic.MUST_NOT:
                    targetQuery = targetQuery.MustNot(curQuery);
                    break;
                default:
                    break;
            }

            return targetQuery;
        }
    }
}