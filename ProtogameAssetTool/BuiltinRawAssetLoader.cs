// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace ProtogameAssetTool
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Protogame;

    /// <summary>
    /// A raw asset loader that loads from the current directory (used for built-in asset loading).
    /// </summary>
    internal class BuiltinRawAssetLoader : IRawAssetLoader
    {
        /// <summary>
        /// The m_ strategies.
        /// </summary>
        private readonly ILoadStrategy[] m_Strategies;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuiltinRawAssetLoader"/> class.
        /// </summary>
        /// <param name="strategies">
        /// The strategies.
        /// </param>
        public BuiltinRawAssetLoader(ILoadStrategy[] strategies)
        {
            this.m_Strategies = strategies;
        }

        /// <summary>
        /// Loads all potential raw asset candidates for the given asset name.  It is up to the
        /// caller to determine which representation will be used.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The raw asset candidates.</returns>
        public IEnumerable<IRawAsset> LoadRawAssetCandidates(string name)
        {
            return this.LoadRawAssetCandidatesWithModificationDates(name).Select(x => x.Key);
        }

        /// <summary>
        /// Loads all potential raw asset candidates for the given asset name, including the last
        /// modification dates of the candidates.  It is up to the caller to determine which
        /// representation will be used.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The raw asset candidates.</returns>
        public IEnumerable<KeyValuePair<IRawAsset, DateTime?>> LoadRawAssetCandidatesWithModificationDates(string name)
        {
            foreach (var strategy in this.m_Strategies)
            {
                DateTime? lastModified = null;
                IRawAsset result = strategy.AttemptLoad(Environment.CurrentDirectory, name, ref lastModified, true);
                if (result != null)
                {
                    yield return new KeyValuePair<IRawAsset, DateTime?>(result, lastModified);
                }
            }
        }

        /// <summary>
        /// Return all available asset names.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> of asset names.
        /// </returns>
        public IEnumerable<string> ScanRawAssets()
        {
            return new[] { "font.Default", "effect.Basic", "effect.Color", "effect.Skinned" };
        }

        public IEnumerable<string> GetPotentialPathsForRawAsset(string name)
        {
            foreach (var strategy in this.m_Strategies)
            {
                var result = strategy.GetPotentialPaths(Environment.CurrentDirectory, name);
                foreach (var r in result)
                {
                    yield return r;
                }
            }
        }
    }
}