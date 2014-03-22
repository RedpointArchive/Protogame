namespace Protogame
{
    using System;
    using System.IO;
    using Protogame.Compression;

    /// <summary>
    /// The local compiled load strategy.
    /// </summary>
    public class LocalCompiledLoadStrategy : ILoadStrategy
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
                return new[] { "bin" };
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
                return false;
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
            var file1 =
                new FileInfo(
                    Path.Combine(
                        path, 
                        TargetPlatformUtility.GetExecutingPlatform().ToString(), 
                        name.Replace('.', Path.DirectorySeparatorChar) + ".bin"));
            var attempt1 = this.AttemptLoadOfFile(file1, name);
            if (attempt1 != null)
            {
                lastModified = file1.LastWriteTime;
                return attempt1;
            }

            var file2 = new FileInfo(Path.Combine(path, name.Replace('.', Path.DirectorySeparatorChar) + ".bin"));
            var attempt2 = this.AttemptLoadOfFile(file2, name);
            if (attempt2 != null)
            {
                lastModified = file2.LastWriteTime;
            }

            return attempt2;
        }

        /// <summary>
        /// The attempt load of file.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// </exception>
        private object AttemptLoadOfFile(FileInfo file, string name)
        {
            if (file.Exists)
            {
                using (var stream = new FileStream(file.FullName, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        if (reader.ReadByte() != 0)
                        {
                            throw new InvalidDataException();
                        }

                        var start = DateTime.Now;
                        using (var memory = new MemoryStream())
                        {
                            LzmaHelper.Decompress(reader.BaseStream, memory);
                            memory.Seek(0, SeekOrigin.Begin);
                            var serializer = new CompiledAssetSerializer();
                            var result = (CompiledAsset)serializer.Deserialize(memory, null, typeof(CompiledAsset));
                            return result;
                        }
                    }
                }
            }

            return null;
        }
    }
}