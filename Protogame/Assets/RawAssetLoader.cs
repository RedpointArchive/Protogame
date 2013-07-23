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
                var file = new FileInfo(
                    Path.Combine(
                        this.m_Path,
                        name.Replace('.', Path.DirectorySeparatorChar) + ".asset"));
                if (file.Exists)
                {
                    using (var reader = new StreamReader(file.FullName, Encoding.UTF8))
                    {
                        var serializer = new JavaScriptSerializer();
                        serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                        return serializer.Deserialize<object>(reader.ReadToEnd());
                    }
                }
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
                        serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                        return serializer.Deserialize<object>(reader.ReadToEnd());
                    }
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
    }
}

