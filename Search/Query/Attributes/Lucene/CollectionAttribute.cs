using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Attributes;
using Query.Expression.FieldValue;

namespace Query.Attributes.Lucene
{
    /// <summary>
    /// collection attribute, for collection query
    /// </summary>
    public class CollectionAttribute: BaseAttribute
    {
        public override void Execute(Common.IContext context)
        {
            var queryContext = context as QueryContext;
            if (queryContext == null)
                return;

            var collection = queryContext.CurrentQuery.FieldValue as Collection;
            var q = LuceneExtension.QueryExtension.QueryParser.ParseCoolection(
                queryContext.CurrentQuery.FieldName,
                collection.ValueCollection);

            queryContext.CurrentQuery.Query = q;
        }
    }
}
