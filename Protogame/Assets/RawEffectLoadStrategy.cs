using System.IO;

namespace Protogame
{
    public class RawEffectLoadStrategy : ILoadStrategy
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
                return new[] { "fx" };
            }
        }

        public object AttemptLoad(string path, string name)
        {
            var file = new FileInfo(
                Path.Combine(
                    path,
                    name.Replace('.', Path.DirectorySeparatorChar) + ".fx"));
            if (file.Exists)
            {
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        return new
                        {
                            Loader = typeof(EffectAssetLoader).FullName,
                            PlatformData = (PlatformData)null,
                            Code = reader.ReadToEnd()
                        };
                    }
                }
            }
            return null;
        }
    }
}
