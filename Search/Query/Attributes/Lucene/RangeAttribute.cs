using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Attributes;
using Query.Expression.FieldValue;
using Common;

namespace Query.Attributes.Lucene
{
    /// <summary>
    /// range attribute, for range query
    /// </summary>
    public class RangeAttribute: BaseAttribute
    {
        public override void Execute(IContext context)
        {
            var queryContext = context as QueryContext;
            if (queryContext == null)
                return;

            var range = queryContext.CurrentQuery.FieldValue as Range;
            var q = LuceneExtension.QueryExtension.QueryParser.ParseRange(
                queryContext.CurrentQuery.FieldName,
                range.Begin,
                range.End,
                range.InClusive);
            queryContext.CurrentQuery.Query = q;
        }
    }
}
