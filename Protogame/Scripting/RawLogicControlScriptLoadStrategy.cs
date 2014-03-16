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
        public object AttemptLoad(string path, string name, ref DateTime? lastModified)
        {
            var file = new FileInfo(Path.Combine(path, name.Replace('.', Path.DirectorySeparatorChar) + ".lc"));
            if (file.Exists)
            {
                lastModified = file.LastWriteTime;
                using (var reader = new StreamReader(file.FullName))
                {
                    return
                        new
                        {
                            Loader = typeof(LogicControlScriptAssetLoader).FullName,
                            Code = reader.ReadToEnd(),
                            SourcedFromRaw = true
                        };
                }
            }

            return null;
        }
    }
}