using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Query.Expression.Node
{
    public class OperationNode: INode
    {
        public OperationNode(QueryLogic logic, INode[] relateionQueries)
        {
            this.RelationQuerys = relateionQueries;
            this.Logic = logic;
        }

        public OperationNode(INode[] relateionQueries) : this(QueryLogic.MUST, relateionQueries) { }

        public QueryType Type
        {
            get
            {
                return QueryType.Operation;
            }
        }

        public QueryLogic Logic { get; set; }

        public object Query { get; set; }

        public INode[] RelationQuerys { get; set; }
    }
}
