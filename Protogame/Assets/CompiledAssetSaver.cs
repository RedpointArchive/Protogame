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
        public void SaveCompiledAsset(string rootPath, string name, IRawAsset data, bool isCompiled)
        {
            var extension = "asset";
            if (isCompiled)
            {
                extension = "bin";
            }

            var file =
                new FileInfo(Path.Combine(rootPath, name.Replace('.', Path.DirectorySeparatorChar) + "." + extension));
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
                    stream.WriteByte(0);
                    using (var memory = new MemoryStream())
                    {
                        var serializer = new CompiledAssetSerializer();
                        serializer.Serialize(memory, compiledData);
                        memory.Seek(0, SeekOrigin.Begin);
                        LzmaHelper.Compress(memory, stream);
                    }
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