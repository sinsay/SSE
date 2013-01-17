using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Attributes
{
    /// <summary>
    /// the command attribute
    /// </summary>
    public class BaseAttribute : Attribute
    {
        public virtual void Execute(IContext context) { }
    }
}
