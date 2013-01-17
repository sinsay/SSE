using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Attributes;
using Query.Expression.FieldValue;

namespace Query.Attributes.Lucene
{
    public class FilterAttribute : BaseAttribute
    {
        public override void Execute(Common.IContext context)
        {
            var queryContext = context as QueryContext;

            if (queryContext == null)
            {
                return;
            }

            var filter = new LuceneExtension.Filter.DuplicateFilter(queryContext.CurrentQuery.FieldName);
            queryContext.Filter = filter;
        }
    }
}
