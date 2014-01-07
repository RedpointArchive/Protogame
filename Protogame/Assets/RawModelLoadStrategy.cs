using System.IO;

namespace Protogame
{
    public class RawModelLoadStrategy : ILoadStrategy
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
                return new[] { "fbx" };
            }
        }

        public object AttemptLoad(string path, string name)
        {
            var file = new FileInfo(
                Path.Combine(
                    path,
                    name.Replace('.', Path.DirectorySeparatorChar) + ".fbx"));
            if (file.Exists)
            {
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var binary = new BinaryReader(fileStream))
                    {
                        return new
                        {
                            Loader = typeof(ModelAssetLoader).FullName,
                            PlatformData = (PlatformData)null,
                            RawData = binary.ReadBytes((int)file.Length),
                            SourcedFromRaw = true
                        };
                    }
                }
            }
            return null;
        }
    }
}
