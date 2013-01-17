using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query.Attributes.Lucene;

namespace Query.Expression.FieldValue
{
    /// <summary>
    /// for immideately value query
    /// </summary>
    [Value]
    public class ImmideatelyValue : IFieldValue
    {
        /// <summary>
        /// immideately value
        /// </summary>
        public string Value { get; set; }

        public ImmideatelyValue(string Value)
        {
            this.Value = Value;
        }
    }
}
