using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Query.Expression.Node
{
    public interface INode
    {
        QueryType Type { get; }

        QueryLogic Logic { get; set; }

        object Query { get; set; }
    }
}
