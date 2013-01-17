using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query.Attributes.Lucene;

namespace Query.Expression.FieldValue.Lucene
{
    [Filter]
    public class DuplicateFilter: IFilter
    {
        public object GetFilter(string fieldName)
        {
            return new LuceneExtension.Filter.DuplicateFilter(fieldName);
        }
    }
}
