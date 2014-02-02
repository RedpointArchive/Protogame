namespace Protogame
{
    using System.Collections.Generic;

    /// <summary>
    /// The RawAssetLoader interface.
    /// </summary>
    public interface IRawAssetLoader
    {
        /// <summary>
        /// Loads the raw representation of an asset from disk or embedded resource.  Returns
        /// all valid assets for the specified name; it is up to the caller to determine which
        /// representation will be used.
        /// </summary>
        /// <returns>
        /// The raw assets.
        /// </returns>
        /// <param name="name">
        /// Name.
        /// </param>
        object[] LoadRawAsset(string name);

        /// <summary>
        /// The scan raw assets.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<string> ScanRawAssets();
    }
}