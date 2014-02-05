namespace Protogame
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The RawAssetLoader interface.
    /// </summary>
    public interface IRawAssetLoader
    {
        /// <summary>
        /// Loads all potential raw asset candidates for the given asset name.  It is up to the
        /// caller to determine which representation will be used.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The raw asset candidates.</returns>
        IEnumerable<object> LoadRawAssetCandidates(string name);

        /// <summary>
        /// Loads all potential raw asset candidates for the given asset name, including the last
        /// modification dates of the candidates.  It is up to the caller to determine which
        /// representation will be used.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The raw asset candidates.</returns>
        IEnumerable<KeyValuePair<object, DateTime?>> LoadRawAssetCandidatesWithModificationDates(string name);

        /// <summary>
        /// Return all available asset names.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> of asset names.
        /// </returns>
        IEnumerable<string> ScanRawAssets();
    }
}