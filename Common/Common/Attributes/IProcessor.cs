using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Attributes
{
    public interface IProcessor
    {
        void Process(IContext context);
    }
}
