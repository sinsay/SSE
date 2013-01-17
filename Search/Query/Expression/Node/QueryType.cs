using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Query.Expression.Node
{
    /// <summary>
    /// query type
    /// </summary>
    public enum QueryType
    {
        QueryField = 1,
        Operation = 2,
        Sort = 4,
        Filter = 8,
    }
}
