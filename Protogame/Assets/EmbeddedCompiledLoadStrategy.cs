using System;
using System.IO;
using System.Linq;
using ProtoBuf;
using Protogame.Compression;

namespace Protogame
{
    public class EmbeddedCompiledLoadStrategy : ILoadStrategy
    {
        public bool ScanSourcePath { get { return false; } }

        public object AttemptLoad(string path, string name)
        {
            var embedded = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                            where !assembly.IsDynamic
                            from resource in assembly.GetManifestResourceNames()
                            where resource == assembly.GetName().Name + "." + name + "-" + TargetPlatformUtility.GetExecutingPlatform().ToString() + ".bin"
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
                        var result = Serializer.Deserialize<CompiledAsset>(memory);
                        Console.WriteLine("Decompressed " + name + " from embedded resource in " + (DateTime.Now - start).TotalSeconds + "s");
                        return result;
                    }
                }
            }
            return null;
        }
    }
}
