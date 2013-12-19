using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ProtoBuf;
using Protogame.Compression;

namespace Protogame
{
    public class RawAssetLoader : IRawAssetLoader
    {
        private string m_Path;
        private ILoadStrategy[] m_Strategies;
    
        public RawAssetLoader(
            ILoadStrategy[] strategies)
        {
            var directory = new DirectoryInfo(Environment.CurrentDirectory).FullName;
            var contentDirectory = Path.Combine(directory, "Content");
            if (Directory.Exists(contentDirectory))
            {
                Directory.CreateDirectory(directory);
                directory = contentDirectory;
            }
            this.m_Path = directory;
            
            // Check for the existance of a .source file in the directory; if so, we
            // set our path to that directory instead.
            if (File.Exists(Path.Combine(this.m_Path, ".source")))
            {
                using (var reader = new StreamReader(Path.Combine(this.m_Path, ".source")))
                {
                    this.m_Path = reader.ReadLine();
                }
            }

            this.m_Strategies = strategies;
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
    }
}

