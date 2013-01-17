using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.Index;

namespace LuceneExtension.QueryExtension
{
    public static class QueryParser
    {
        private static Dictionary<string, Lucene.Net.QueryParsers.QueryParser> parserCache;

        static QueryParser()
        {
            parserCache = new Dictionary<string,Lucene.Net.QueryParsers.QueryParser> ();
        }

        /// <summary>
        /// get parser by fieldname
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static Lucene.Net.QueryParsers.QueryParser GetParser(string fieldName)
        {
            lock (parserCache)
            {
                if (!parserCache.ContainsKey(fieldName))
                {
                    // TODO: 将版本号抽离，统一定义
                    parserCache[fieldName] = new Lucene.Net.QueryParsers.QueryParser(
                        Lucene.Net.Util.Version.LUCENE_29, 
                        fieldName, 
                        new LuceneAnalyser());
                }
            }

            return parserCache[fieldName];
        }

        public static BooleanQuery ParseCoolection(string fieldName, string[] values)
        {
            var parser = GetParser(fieldName);
            var query = new BooleanQuery();

            foreach (var value in values)
            {
                var q = parser.Parse(value);
                if (q != null)
                {
                    query = query.Should(q);
                }
            }

            return query;
        }

        public static BooleanQuery ParseRange(string fieldName, long lowerValue, long upperValue, bool inclusive)
        {
            if (lowerValue > upperValue)
            {
                return null;
            }

            //var rangeQuery = new BooleanQuery();
            var dateQuery = new BooleanQuery();
            BooleanQuery.SetMaxClauseCount(int.MaxValue);

            for (long i = lowerValue; i < upperValue; i++)
            {
                var term = new Term(fieldName, i.ToString());
                var q = new TermQuery(term);
                dateQuery.Add(q, BooleanClause.Occur.SHOULD);
            }

            if (inclusive)
            {
                var term = new Term(fieldName, upperValue.ToString());
                var q = new TermQuery(term);
                dateQuery.Add(q, BooleanClause.Occur.SHOULD);
            }

            //if (dateQuery.GetClauses() != null || dateQuery.GetClauses().Length != 0)
            //{
            //    rangeQuery.Add(dateQuery, BooleanClause.Occur.MUST);
            //}

            return dateQuery;
        }

        public static Query Parse(string fieldName, string fieldValue)
        {
            var parser = GetParser(fieldName);
            var q = parser.Parse(fieldValue);
            return q;
        }

        public static BooleanQuery Must(this BooleanQuery self, Query other)
        {
            return LogicOperator(self, other, Lucene.Net.Search.BooleanClause.Occur.MUST);
        }

        public static BooleanQuery Should(this BooleanQuery self, Query other)
        {
            return LogicOperator(self, other, Lucene.Net.Search.BooleanClause.Occur.SHOULD);
        }

        public static BooleanQuery MustNot(this BooleanQuery self, Query other)
        {
            return LogicOperator(self, other, Lucene.Net.Search.BooleanClause.Occur.MUST_NOT);
        }

        private static BooleanQuery LogicOperator(BooleanQuery self, Query other, BooleanClause.Occur occur)
        {
            var query = self;
            if(query == null || query.GetClauses() == null || query.GetClauses().Length == 0)
                query = new BooleanQuery();

            if (other != null)
            {
                query.Add(other, occur);
            }

            return query;
        }
    }
}
