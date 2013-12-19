using System.Collections.Generic;

namespace Protogame
{
    public interface IRawAssetLoader
    {
        IEnumerable<string> ScanRawAssets();

        /// <summary>
        /// Loads the raw representation of an asset from disk or embedded resource.  Returns
        /// all valid assets for the specified name; it is up to the caller to determine which
        /// representation will be used.
        /// </summary>
        /// <returns>The raw assets.</returns>
        /// <param name="name">Name.</param>
        object[] LoadRawAsset(string name);
    }
}

