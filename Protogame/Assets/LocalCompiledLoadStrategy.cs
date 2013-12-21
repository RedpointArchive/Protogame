using System;
using System.IO;
using ProtoBuf;
using Protogame.Compression;

namespace Protogame
{
    public class LocalCompiledLoadStrategy : ILoadStrategy
    {
        public bool ScanSourcePath
        {
            get
            {
                return false;
            }
        }

        public string[] AssetExtensions
        {
            get
            {
                return new[] { "bin" };
            }
        }

        public object AttemptLoad(string path, string name)
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
                return attempt1;
            }

            var file2 =
                new FileInfo(
                    Path.Combine(
                        path,
                        name.Replace('.', Path.DirectorySeparatorChar) + ".bin"));
            return this.AttemptLoadOfFile(file2, name);
        }

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
                            var result = Serializer.Deserialize<CompiledAsset>(memory);
                            Console.WriteLine("Decompressed " + name + " from file in " + (DateTime.Now - start).TotalSeconds + "s");
                            return result;
                        }
                    }
                }
            }
            return null;
        }
    }
}
