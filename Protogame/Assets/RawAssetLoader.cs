using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Protogame
{
    public class RawAssetLoader : IRawAssetLoader
    {
        private string m_Path;
        private List<ILoadStrategy> m_Strategies;
    
        public RawAssetLoader()
        {
            var directory = new DirectoryInfo(Environment.CurrentDirectory).FullName;
            var contentDirectory = Path.Combine(directory, "Content");
            if (Directory.Exists(contentDirectory))
            {
                Directory.CreateDirectory(directory);
                directory = contentDirectory;
            }
            this.m_Path = directory;
            
            this.m_Strategies = new List<ILoadStrategy>();
            this.m_Strategies.Add(new LocalLoadStrategy());
            #if DEBUG
            this.m_Strategies.Add(new RawTextureLoadStrategy());
            #endif
            this.m_Strategies.Add(new EmbeddedLoadStrategy());
        }
    
        public IEnumerable<string> RescanAssets(string path, string prefixes = "")
        {
            var directoryInfo = new DirectoryInfo(path + "/" + prefixes.Replace('.', '/'));
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.Extension != ".asset")
                    continue;
                var name = file.Name.Substring(0, file.Name.Length - ".asset".Length);
                var asset = (prefixes.Trim('.') + "." + name).Trim('.');
                yield return asset;
            }
            foreach (var directory in directoryInfo.GetDirectories())
            {
                foreach (var asset in this.RescanAssets(path, prefixes + directory.Name + "."))
                    yield return asset;
            }
        }
        
        public IEnumerable<string> ScanRawAssets()
        {
            return this.RescanAssets(this.m_Path);
        }
        
        public object LoadRawAsset(string name)
        {
            try
            {
                foreach (var strategy in this.m_Strategies)
                {
                    var result = strategy.AttemptLoad(this.m_Path, name);
                    if (result != null)
                        return result;
                }
                throw new AssetNotFoundException(name);
            }
            catch (Exception ex)
            {
                if (ex is AssetNotFoundException)
                    throw;
                throw new AssetNotFoundException(name, ex);
            }
        }
        
        private interface ILoadStrategy
        {
            object AttemptLoad(string path, string name);
        }
        
        private class LocalLoadStrategy : ILoadStrategy
        {
            public object AttemptLoad(string path, string name)
            {
                var file = new FileInfo(
                    Path.Combine(
                        path,
                        name.Replace('.', Path.DirectorySeparatorChar) + ".asset"));
                if (file.Exists)
                {
                    using (var reader = new StreamReader(file.FullName, Encoding.UTF8))
                    {
                        var serializer = new JavaScriptSerializer();
                        serializer.MaxJsonLength = Int32.MaxValue;
                        serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                        return serializer.Deserialize<object>(reader.ReadToEnd());
                    }
                }
                return null;
            }
        }
        
        private class EmbeddedLoadStrategy : ILoadStrategy
        {
            public object AttemptLoad(string path, string name)
            {
                var embedded = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                where !assembly.IsDynamic
                                from resource in assembly.GetManifestResourceNames()
                                where resource == assembly.GetName().Name + "." + name + ".asset"
                                select assembly.GetManifestResourceStream(resource)).ToList();
                if (embedded.Any())
                {
                    using (var reader = new StreamReader(embedded.First(), Encoding.UTF8))
                    {
                        var serializer = new JavaScriptSerializer();
                        serializer.MaxJsonLength = Int32.MaxValue;
                        serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                        return serializer.Deserialize<object>(reader.ReadToEnd());
                    }
                }
                return null;
            }
        }
        
        #if DEBUG
            
        private class RawTextureLoadStrategy : ILoadStrategy
        {
            public object AttemptLoad(string path, string name)
            {
                var file = new FileInfo(
                    Path.Combine(
                        path,
                        name.Replace('.', Path.DirectorySeparatorChar) + ".png"));
                if (file.Exists)
                {
                    using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        using (var binary = new BinaryReader(fileStream))
                        {
                            return new {
                                Loader = typeof(TextureAssetLoader).FullName,
                                SourcePath = file.FullName,
                                TextureData = binary.ReadBytes((int)file.Length)
                            };
                        }
                    }
                }
                return null;
            }
        }
        
        #endif
    }
}

