namespace Protogame
{
    using System.IO;

    /// <summary>
    /// The raw texture load strategy.
    /// </summary>
    public class RawTextureLoadStrategy : ILoadStrategy
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
                return new[] { "png" };
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
        public object AttemptLoad(string path, string name)
        {
            var file = new FileInfo(Path.Combine(path, name.Replace('.', Path.DirectorySeparatorChar) + ".png"));
            if (file.Exists)
            {
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var binary = new BinaryReader(fileStream))
                    {
                        return
                            new
                            {
                                Loader = typeof(TextureAssetLoader).FullName, 
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