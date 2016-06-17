using System;
using System.Collections.Generic;
using System.IO;

namespace Protogame
{
    public class RawUserInterfaceLoadStrategy : ILoadStrategy
    {
        public string[] AssetExtensions
        {
            get { return new[] { "ui2" }; }
        }

        public bool ScanSourcePath
        {
            get { return true; }
        }

        public IRawAsset AttemptLoad(string path, string name, ref DateTime? lastModified, bool noTranslate = false)
        {
            var file = new FileInfo(Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".ui2"));
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
                                    Loader = typeof(UserInterfaceAssetLoader).FullName,
                                    PlatformData = (PlatformData)null,
                                    UserInterfaceData = reader.ReadToEnd(),
                                    UserInterfaceFormat = UserInterfaceFormat.XmlVersion2,
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
            yield return Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".ui2");
        }
    }
}
