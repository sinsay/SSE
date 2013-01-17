using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query.Expression;
using Common;

namespace Query.Processor
{
    public interface IQueryProcessor
    {
        /// <summary>
        /// build qinfo's query with filed name and value
        /// </summary>
        /// <param name="qinfo"></param>
        /// <returns></returns>
        void Execute(IContext qinfo);

        /// <summary>
        /// set the qinfo's boost
        /// </summary>
        /// <param name="qinfo"></param>
        /// <returns></returns>
        //QueryInfo SetBoost(QueryInfo qinfo);
    }
}
