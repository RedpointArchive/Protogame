namespace Protogame
{
    using System;
    using System.IO;

    /// <summary>
    /// The load strategy for loading LogicScript assets from raw files.
    /// </summary>
    public class RawLogicControlScriptLoadStrategy : ILoadStrategy
    {
        /// <summary>
        /// Gets the file extensions of files on disk that map to this load strategy.
        /// </summary>
        /// <value>
        /// The file extensions.
        /// </value>
        public string[] AssetExtensions
        {
            get
            {
                return new[] { "lc" };
            }
        }

        /// <summary>
        /// Gets a value indicating whether the asset source path should be scanned.
        /// </summary>
        /// <value>
        /// Whether or not the asset source path should be scanned.
        /// </value>
        public bool ScanSourcePath
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Attempt to load the asset from the raw file.
        /// </summary>
        /// <param name="path">The base path that assets are being loaded from.</param>
        /// <param name="name">The name of the asset.</param>
        /// <param name="lastModified">The date and time that the asset was last modified.</param>
        /// <returns>The anonymous object to be loaded by <see cref="LogicControlScriptAssetLoader"/> or null.</returns>
        public IRawAsset AttemptLoad(string path, string name, ref DateTime? lastModified, bool noTranslate = false)
        {
            var file = new FileInfo(Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".lc"));
            if (file.Exists)
            {
                lastModified = file.LastWriteTime;
                using (var reader = new StreamReader(file.FullName))
                {
                    return
                        new AnonymousObjectBasedRawAsset(
                            new
                            {
                                Loader = typeof(LogicControlScriptAssetLoader).FullName,
                                Code = reader.ReadToEnd(),
                                SourcedFromRaw = true
                            });
                }
            }

            return null;
        }

        public System.Collections.Generic.IEnumerable<string> GetPotentialPaths(string path, string name, bool noTranslate = false)
        {
            yield return Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".lc");
        }
    }
}