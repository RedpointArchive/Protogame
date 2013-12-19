using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame
{
    public interface ILoadStrategy
    {
        bool ScanSourcePath { get; }

        object AttemptLoad(string path, string name);
    }
}
