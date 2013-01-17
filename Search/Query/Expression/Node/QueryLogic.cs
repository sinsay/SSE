using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Query.Expression.Node
{
    /// <summary>
    /// query logic
    /// </summary>
    public enum QueryLogic
    {
        MUST = 1,
        SHOULD = 2,
        MUST_NOT = 4
    }
}
