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
        private string m_SourcePath;
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
            this.m_SourcePath = directory;
            
            // Check for the existance of a .source file in the directory; if so, we
            // also search that path.
            if (File.Exists(Path.Combine(this.m_Path, ".source")))
            {
                using (var reader = new StreamReader(Path.Combine(this.m_Path, ".source")))
                {
                    this.m_SourcePath = reader.ReadLine();
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
        
        public object[] LoadRawAsset(string name)
        {
            var candidates = new List<object>();
            foreach (var strategy in this.m_Strategies)
            {
                object result;

                if (strategy.ScanSourcePath && this.m_SourcePath != null)
                {
                    result = strategy.AttemptLoad(this.m_SourcePath, name);
                    if (result != null)
                        candidates.Add(result);
                }

                result = strategy.AttemptLoad(this.m_Path, name);
                if (result != null)
                    candidates.Add(result);
            }
            return candidates.ToArray();
        }
    }
}

