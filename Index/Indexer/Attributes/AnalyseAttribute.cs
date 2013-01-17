using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Indexer.Attributes
{
    /// <summary>
    /// attribute to set analyse
    /// </summary>
    [UtilityLib.Reflection.Priority(1)]
    public class AnalyseAttribute : Common.Attributes.BaseAttribute
    {
        public override void Execute(Common.IContext context)
        {
            var indexContext = context as IndexContext;
            indexContext.CurrentFieldInfo.Analyse = true;
        }
    }
}
