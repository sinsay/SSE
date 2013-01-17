using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuceneAddin = Lucene.Net.Search;
using Indexer.IndexInterface.LuceneInterface;
using Indexer.Attributes;

namespace Searcher.Processor.Lucene
{
    public class LuceneSearchProcessor : ISearchProcessor
    {
        public void Execute<T>(Common.IContext context)
        {
            var searchContext = context as SearchContext<T>;
            if (searchContext == null)
                return;

            // TODO: 增加缓存
            var searcher = GetSearcher<T>(searchContext);

            LuceneAddin.Hits hits = searchContext.QueryContext.Filter == null ?
                searcher.Search(
                    searchContext.QueryContext.Query as LuceneAddin.Query,
                    searchContext.QueryContext.Sort as LuceneAddin.Sort) :
                searcher.Search(searchContext.QueryContext.Query as LuceneAddin.Query,
                searchContext.QueryContext.Filter as LuceneAddin.Filter,
                searchContext.QueryContext.Sort as LuceneAddin.Sort);

            var hitsLength = hits.Length();
            var startIndex = (searchContext.PageIndex - 1) * searchContext.PageSize;

            // 获取所有被存储了的字段名
            var storeProps = GetFieldNameWithStoreAttr<T>();
            var tList = new List<T>(searchContext.PageSize);
            for (int i = startIndex; i < hitsLength; i++)
            {
                // TODO: 根据加了唯一特性的Property来过滤
                T t = UtilityLib.Reflection.Reflection.CreateInstance<T>(null);

                var doc = hits.Doc(i);
                foreach (var prop in storeProps)
                {
                    var name = prop.Name;
                    var field = doc.GetField(name);
                    var value = field.StringValue();
                    SetItemValue<T>(t, prop, value);
                }

                tList.Add(t);

                if (tList.Count >= searchContext.PageSize)
                    break;
            }

            searchContext.Result = new SearchResult<T>()
            {
                Documents = tList.ToArray(),
                Count = hitsLength,
                TotalMillseconds = 0
            };
        }

        /// <summary>
        /// 获取被存储了的字段的值，并转换为实体的形式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        private void SetItemValue<T>(T t, System.Reflection.PropertyInfo prop, string value)
        {
            var changetableAttrs = UtilityLib.Reflection.Attribute.GetAttributes<ChangeableAttribute>(t, prop);
            if (changetableAttrs != null && changetableAttrs.Length > 0)
            {
                foreach (var changeable in changetableAttrs)
                {
                    changeable.GetChange(t, prop, value);
                }
            }
            else
            {
                UtilityLib.Reflection.Property.SetValue(t, prop, value);
            }
        }

        /// <summary>
        /// 获取类型为T的且索引时被存储了的字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private System.Reflection.PropertyInfo[] GetFieldNameWithStoreAttr<T>()
        {
            var props = UtilityLib.Reflection.Property.GetProperties(typeof(T));
            var storeAttrs = new List<System.Reflection.PropertyInfo>();
            var instanceOfType = UtilityLib.Reflection.Reflection.CreateInstance<T>(null);

            foreach (var prop in props)
            {
                if (UtilityLib.Reflection.Attribute.HasAttribute(instanceOfType, prop, typeof(StroeAttribute)))
                {
                    storeAttrs.Add(prop);
                }
            }

            return storeAttrs.ToArray();
        }

        /// <summary>
        /// 获取检索器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchContext"></param>
        /// <returns></returns>
        private static LuceneAddin.Searcher GetSearcher<T>(SearchContext<T> searchContext)
        {
            // 从缓存获取检索器
            var pathInfo = LuceneSearcherManager.GetRange(
                searchContext.Pathes.Select(d => d.IndexPath).ToArray());

            LuceneAddin.IndexSearcher[] searchables = pathInfo.Useful;

            LuceneAddin.Searcher searcher;

            if (searchables.Length == 1)
            {
                searcher = searchables[0];
            }
            else if (searchables.Length > 1)
            {
                searcher = new LuceneAddin.ParallelMultiSearcher(searchables.ToArray());
            }
            else
            {
                throw new Exception("search with empty path...");
            }

            return searcher;
        }
    }
}