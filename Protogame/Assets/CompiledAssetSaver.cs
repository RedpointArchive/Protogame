using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ProtoBuf;
using Protogame.Compression;

namespace Protogame
{
    public class CompiledAssetSaver
    {
        public void SaveCompiledAsset(string rootPath, string name, object data, bool isCompiled)
        {
            var extension = "asset";
            if (isCompiled)
            {
                extension = "bin";
            }
            var file = new FileInfo(
                Path.Combine(
                    rootPath,
                    name.Replace('.', Path.DirectorySeparatorChar) + "." + extension));
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
                    stream.WriteByte((byte)0);
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
                    writer.Write(JsonConvert.SerializeObject(data));
                }
            }
        }

        private void CreateDirectories(DirectoryInfo directory)
        {
            if (directory.Exists)
                return;
            this.CreateDirectories(directory.Parent);
            directory.Create();
        }
    }
}
