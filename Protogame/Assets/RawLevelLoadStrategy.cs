using System.IO;

namespace Protogame
{
    public class RawLevelLoadStrategy : ILoadStrategy
    {
        public bool ScanSourcePath
        {
            get
            {
                return true;
            }
        }

        public string[] AssetExtensions
        {
            get
            {
                return new[] { "oel" };
            }
        }

        public object AttemptLoad(string path, string name)
        {
            var file = new FileInfo(
                Path.Combine(
                    path,
                    name.Replace('.', Path.DirectorySeparatorChar) + ".oel"));
            if (file.Exists)
            {
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        return new
                        {
                            Loader = typeof(LevelAssetLoader).FullName,
                            PlatformData = (PlatformData)null,
                            Value = reader.ReadToEnd(),
                            SourcePath = (string)null,
                            SourcedFromRaw = true
                        };
                    }
                }
            }
            return null;
        }
    }
}
