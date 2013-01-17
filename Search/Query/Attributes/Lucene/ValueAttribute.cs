using Common;
using Common.Attributes;
using Query.Expression.FieldValue;
using LuceneExtension.QueryExtension;

namespace Query.Attributes.Lucene
{

    /// <summary>
    /// get value as query expression immediately
    /// </summary>
    public class ValueAttribute: BaseAttribute
    {
        public override void Execute(IContext context)
        {
            var queryContext = context as QueryContext;
            if (queryContext == null)
                return;

            var valueInfo = queryContext.CurrentQuery.FieldValue as ImmideatelyValue;
            var q = QueryParser.Parse(
                queryContext.CurrentQuery.FieldName,
                valueInfo.Value);

            queryContext.CurrentQuery.Query =  q;
        }
    }
}
