using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Shell
{
    public interface IExecutable
    {
        string Cmd { get; }

        string Name { get; }

        void Execute(IEnumerable<string> args);
    }
}
