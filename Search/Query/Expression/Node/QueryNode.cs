using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query.Expression.FieldValue;

namespace Query.Expression.Node
{
    /// <summary>
    /// query info, contains the tree of query condition
    /// </summary>
    public class QueryNode
    {
        #region Constructor

        public QueryNode(string fieldName, IFieldValue fieldValue, QueryLogic logic, QueryType type)
        {
            this.FieldName = fieldName;
            this.FieldValue = fieldValue;
            this.Logic = logic;
            this.Type = type;
        }

        /// <summary>
        /// initialize an query node
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="logic"></param>
        /// <param name="type"></param>
        public QueryNode(string fieldName, IFieldValue fieldValue, QueryLogic logic) 
            : this(fieldName, fieldValue, logic, QueryType.QueryField) { }

        /// <summary>
        /// initialize an query node
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        public QueryNode(string fieldName, IFieldValue fieldValue) 
            : this(fieldName, fieldValue, QueryLogic.MUST,  QueryType.QueryField) { }

        /// <summary>
        /// initiaize an operation node
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="relationNodes"></param>
        public QueryNode(QueryLogic logic, QueryNode[] relationNodes) :
            this(string.Empty, null, logic, QueryType.Operation)
        {
            this.RelationQuerys = relationNodes;
        }

        /// <summary>
        /// initialize an operation node
        /// </summary>
        /// <param name="relationNodes"></param>
        public QueryNode(QueryNode[] relationNodes) : this(QueryLogic.MUST, relationNodes) { }

        #endregion

        /// <summary>
        /// the query field's name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// the query value, it can be many types of value. see Query.Expression.FieldValue
        /// </summary>
        public IFieldValue FieldValue { get; set; }

        /// <summary>
        /// the final query condition
        /// </summary>
        public object Query { get; set; }

        /// <summary>
        /// query logic, like MUST and SHOULD
        /// </summary>
        public QueryLogic Logic{get;set;}

        /// <summary>
        /// if this instance is an operation node, it means the sub query node
        /// </summary>
        public QueryNode[] RelationQuerys { get; set; }

        /// <summary>
        /// type of this node
        /// </summary>
        public QueryType Type { get; set; }


        #region Factory Method

        /// <summary>
        /// craete an query node
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="logic"></param>
        /// <returns></returns>
        public static QueryNode CreateQueryNode(string fieldName, IFieldValue fieldValue, QueryLogic logic)
        {
            return new QueryNode(fieldName, fieldValue, logic);
        }

        /// <summary>
        /// create an operation node
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="relateNodes"></param>
        /// <returns></returns>
        public static QueryNode CreateOperNode(QueryLogic logic, QueryNode[] relateNodes)
        {
            return new QueryNode(logic, relateNodes);
        }

        public static QueryNode CreateFilterNode(string fieldName, IFieldValue fieldValue)
        {
            return new QueryNode(fieldName, fieldValue) {  Type = QueryType.Filter };
        }

        public static QueryNode CreateSortNode(string fieldName, bool desc)
        {
            return new QueryNode(fieldName, new Sort(desc)) {  Type = QueryType.Sort};
        }

        #endregion
    }
}