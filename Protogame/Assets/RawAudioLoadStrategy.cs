namespace Protogame
{
    using System;
    using System.IO;

    /// <summary>
    /// The raw audio load strategy.
    /// </summary>
    public class RawAudioLoadStrategy : ILoadStrategy
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
                return new[] { "wav" };
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
        public object AttemptLoad(string path, string name, ref DateTime? lastModified)
        {
            var file = new FileInfo(Path.Combine(path, name.Replace('.', Path.DirectorySeparatorChar) + ".wav"));
            if (file.Exists)
            {
                lastModified = file.LastWriteTime;
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var binary = new BinaryReader(fileStream))
                    {
                        return
                            new
                            {
                                Loader = typeof(AudioAssetLoader).FullName, 
                                PlatformData = (PlatformData)null, 
                                RawData = binary.ReadBytes((int)file.Length), 
                                SourcedFromRaw = true
                            };
                    }
                }
            }

            return null;
        }
    }
}