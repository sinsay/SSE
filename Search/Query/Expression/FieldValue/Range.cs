using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query.Attributes.Lucene;

namespace Query.Expression.FieldValue
{
    /// <summary>
    /// for range query
    /// </summary>
    [Range]
    public class Range: IFieldValue
    {
        /// <summary>
        /// the range start with 
        /// </summary>
        public long Begin { get; set; }

        /// <summary>
        /// the range end with
        /// </summary>
        public long End { get; set; }

        /// <summary>
        /// it's clusive the edge
        /// </summary>
        public bool InClusive { get; set; }

        public Range(long begin, long end, bool inClusive)
        {
            this.Begin = begin;
            this.End = end;
            this.InClusive = inClusive;
        }
    }
}
