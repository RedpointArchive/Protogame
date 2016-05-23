using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protogame
{
    public class RawATFLevelLoadStrategy : ILoadStrategy
    {
        public string[] AssetExtensions
        {
            get { return new[] {"lvl"}; }
        }

        public bool ScanSourcePath
        {
            get { return true; }
        }

        public IRawAsset AttemptLoad(string path, string name, ref DateTime? lastModified, bool noTranslate = false)
        {
            var file = new FileInfo(Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".lvl"));
            if (file.Exists)
            {
                lastModified = file.LastWriteTime;
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        return
                            new AnonymousObjectBasedRawAsset(
                                new
                                {
                                    Loader = typeof(LevelAssetLoader).FullName,
                                    PlatformData = (PlatformData)null,
                                    LevelData = reader.ReadToEnd(),
                                    LevelDataFormat = LevelDataFormat.ATF,
                                    SourcePath = (string)null,
                                    SourcedFromRaw = true
                                });
                    }
                }
            }

            return null;
        }

        public IEnumerable<string> GetPotentialPaths(string path, string name, bool noTranslate = false)
        {
            yield return Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".lvl");
        }
    }
}
