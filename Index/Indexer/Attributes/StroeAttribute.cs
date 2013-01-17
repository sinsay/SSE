using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Indexer;
using Common;
using Common.Attributes;

namespace Indexer.Attributes
{
    /// <summary>
    /// attribute for set the store property
    /// </summary>
    [UtilityLib.Reflection.Priority(1)]
    public class StroeAttribute: BaseAttribute
    {
        public override void Execute(IContext context)
        {
            var indexContext = context as IndexContext;
            indexContext.CurrentFieldInfo.Store = true;
        }
    }
}
