using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilityLib.Reflection
{
    public class PriorityAttribute : System.Attribute
    {
        public int Priotiry { get; set; }

        public PriorityAttribute(int priority)
        {
            this.Priotiry = priority;
        }
    }
}
