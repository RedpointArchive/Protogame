namespace Protogame
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using Protogame.Compression;

    /// <summary>
    /// The compiled asset saver.
    /// </summary>
    public class CompiledAssetSaver
    {
        /// <summary>
        /// The save compiled asset.
        /// </summary>
        /// <param name="rootPath">
        /// The root path.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="isCompiled">
        /// The is compiled.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void SaveCompiledAsset(string rootPath, string name, IRawAsset data, bool isCompiled, string embedPlatform = null)
        {
            var extension = "asset";
            if (isCompiled)
            {
                extension = "bin";
            }

            var filename = name.Replace('.', Path.DirectorySeparatorChar) + "." + extension;
            if (!string.IsNullOrWhiteSpace(embedPlatform))
            {
                filename = name + "-" + embedPlatform + "." + extension;
            }

            var file = new FileInfo(Path.Combine(rootPath, filename));
            this.CreateDirectories(file.Directory);
            if (isCompiled)
            {
                if (!(data is CompiledAsset))
                {
                    throw new InvalidOperationException();
                }

                var compiledData = (CompiledAsset)data;
                using (var stream = new FileStream(file.FullName, FileMode.Create))
                {
                    // LZMA compression is proving too expensive to decompress.  We'll look into
                    // LZ4 in the future, but for now, just store assets uncompressed.
                    stream.WriteByte(CompiledAsset.FORMAT_UNCOMPRESSED);

                    var serializer = new CompiledAssetSerializer();
                    serializer.Serialize(stream, compiledData);
                }
            }
            else
            {
                using (var writer = new StreamWriter(file.FullName, false, Encoding.UTF8))
                {
                    writer.Write(JsonConvert.SerializeObject(data.Properties.ToDictionary(k => k.Key, v => v.Value)));
                }
            }
        }

        /// <summary>
        /// The create directories.
        /// </summary>
        /// <param name="directory">
        /// The directory.
        /// </param>
        private void CreateDirectories(DirectoryInfo directory)
        {
            if (directory.Exists)
            {
                return;
            }

            this.CreateDirectories(directory.Parent);
            directory.Create();
        }
    }
}