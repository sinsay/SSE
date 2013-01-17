using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Query.Expression.FieldValue
{
    [Query.Attributes.Lucene.Sort]
    public class Sort: IFieldValue
    {
        public bool Desc { get; set; }

        public Sort(bool desc)
        {
            this.Desc = desc;
        }
    }
}
