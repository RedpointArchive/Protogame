#if PLATFORM_ANDROID || PLATFORM_OUYA
namespace Protogame
{
    using System;
    using System.IO;
    using Protogame.Compression;

    /// <summary>
    /// The Android compiled load strategy.
    /// </summary>
    public class AndroidCompiledLoadStrategy : ILoadStrategy
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
        public IRawAsset AttemptLoad(string path, string name, ref DateTime? lastModified, bool noTranslate = false)
        {
            try
            {
                var stream1 = global::Android.App.Application.Context.Assets.Open(
                                  TargetPlatformUtility.GetExecutingPlatform().ToString() + Path.DirectorySeparatorChar +
                                  (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".bin");
                return this.AttemptLoadOfStream(stream1, name);
            } catch (Java.IO.FileNotFoundException)
            {
                try
                {
                    var stream2 = global::Android.App.Application.Context.Assets.Open(
                                      (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".bin");
                    return this.AttemptLoadOfStream(stream2, name);
                } catch (Java.IO.FileNotFoundException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The attempt load of stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// </exception>
        private IRawAsset AttemptLoadOfStream(Stream stream, string name)
        {
            if (stream == null)
            {
                return null;
            }

            using (var copy = new MemoryStream())
            {
                // The reader.BaseStream won't support the Length property as it
                // is a stream from an Android asset; we have to copy the stream
                // to memory and then decompress it.
                stream.CopyTo(copy);
                copy.Seek(0, SeekOrigin.Begin);

                using (var reader = new BinaryReader(copy))
                {
                    switch (reader.ReadByte())
                    {
                        case CompiledAsset.FORMAT_LZMA_COMPRESSED:
                            using (var memory = new MemoryStream())
                            {
                                LzmaHelper.Decompress(reader.BaseStream, memory);
                                memory.Seek(0, SeekOrigin.Begin);
                                var serializer = new CompiledAssetSerializer();
                                var result = (CompiledAsset)serializer.Deserialize(memory, null, typeof(CompiledAsset));
                                return result;
                            }
                        case CompiledAsset.FORMAT_UNCOMPRESSED:
                            var ucserializer = new CompiledAssetSerializer();
                            var ucresult = (CompiledAsset)ucserializer.Deserialize(reader.BaseStream, null, typeof(CompiledAsset));
                            return ucresult;
                        default:
                            throw new InvalidDataException();
                    }
                }
            }
        }

        public System.Collections.Generic.IEnumerable<string> GetPotentialPaths(string path, string name, bool noTranslate = false)
        {
            yield break;
        }
    }
}
#endif