namespace Protogame
{
    using System;
    using System.IO;

    /// <summary>
    /// The raw level load strategy.
    /// </summary>
    public class RawLevelLoadStrategy : ILoadStrategy
    {
        /// <summary>
        /// Gets the asset extensions.
        /// </summary>
        /// <value>
        /// The asset extensions.
        /// </value>
        public string[] AssetExtensions
        {
            get
            {
                return new[] { "oel" };
            }
        }

        /// <summary>
        /// Gets a value indicating whether scan source path.
        /// </summary>
        /// <value>
        /// The scan source path.
        /// </value>
        public bool ScanSourcePath
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The attempt load.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public IRawAsset AttemptLoad(string path, string name, ref DateTime? lastModified)
        {
            var file = new FileInfo(Path.Combine(path, name.Replace('.', Path.DirectorySeparatorChar) + ".oel"));
            if (file.Exists)
            {
                lastModified = file.LastWriteTime;
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        return
                            new AnonymousObjectBasedRawAsset(
                                new
                                {
                                    Loader = typeof(LevelAssetLoader).FullName,
                                    PlatformData = (PlatformData)null,
                                    Value = reader.ReadToEnd(),
                                    SourcePath = (string)null,
                                    SourcedFromRaw = true
                                });
                    }
                }
            }

            return null;
        }
    }
}