using System.Collections.Generic;

namespace Protogame
{
    public interface IRawAssetLoader
    {
        IEnumerable<string> ScanRawAssets();
    
        object LoadRawAsset(string name);
    }
}

