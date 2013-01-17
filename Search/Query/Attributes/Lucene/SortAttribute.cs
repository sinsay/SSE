using Common;
using Common.Attributes;
using LuceneAddIn = Lucene.Net;

namespace Query.Attributes.Lucene
{
    public class SortAttribute: BaseAttribute
    {
        public override void Execute(IContext context)
        {
            var queryContext = context as QueryContext;

            if (queryContext == null)
                return;

            var sort = queryContext.CurrentQuery.FieldValue as Query.Expression.FieldValue.Sort;
            queryContext.Sort = new LuceneAddIn.Search.Sort(
                // TODO: 这里暂时强制使用整型排序
                new LuceneAddIn.Search.SortField(queryContext.CurrentQuery.FieldName, LuceneAddIn.Search.SortField.INT, sort.Desc));
        }
    }
}
