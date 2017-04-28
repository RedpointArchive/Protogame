#if FALSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtogameAssetTool
{
    interface IAssetOutOfDateCalculator
    {
        string[] GetAssetsForRecompilation(string outputRootPath);
    }
}

#endif