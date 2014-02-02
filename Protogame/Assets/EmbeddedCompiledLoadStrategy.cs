namespace Protogame
{
    using System;
    using System.IO;
    using System.Linq;
    using Protogame.Compression;

    /// <summary>
    /// The embedded compiled load strategy.
    /// </summary>
    public class EmbeddedCompiledLoadStrategy : ILoadStrategy
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
        /// <exception cref="InvalidDataException">
        /// </exception>
        public object AttemptLoad(string path, string name)
        {
            var embedded = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                            where !assembly.IsDynamic
                            from resource in assembly.GetManifestResourceNames()
                            where
                                resource
                                == assembly.GetName().Name + "." + name + "-"
                                + TargetPlatformUtility.GetExecutingPlatform() + ".bin"
                            select assembly.GetManifestResourceStream(resource)).ToList();
            if (embedded.Any())
            {
                using (var reader = new BinaryReader(embedded.First()))
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
                        Console.WriteLine(
                            "Decompressed " + name + " from embedded resource in " + (DateTime.Now - start).TotalSeconds
                            + "s");
                        return result;
                    }
                }
            }

            return null;
        }
    }
}