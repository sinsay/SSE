using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query.Attributes.Lucene;

namespace Query.Expression.FieldValue
{
    /// <summary>
    /// collection value, for collection query
    /// </summary>
    [Collection]
    public class Collection: IFieldValue
    {
        /// <summary>
        /// the collection of query value
        /// </summary>
        public string[] ValueCollection { get; set; }

        public Collection(string[] valueCollection)
        {
            this.ValueCollection = valueCollection;
        }
    }
}
