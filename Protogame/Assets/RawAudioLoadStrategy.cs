using System.IO;

namespace Protogame
{
    public class RawAudioLoadStrategy : ILoadStrategy
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
                return new[] { "wav" };
            }
        }

        public object AttemptLoad(string path, string name)
        {
            var file = new FileInfo(
                Path.Combine(
                    path,
                    name.Replace('.', Path.DirectorySeparatorChar) + ".wav"));
            if (file.Exists)
            {
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var binary = new BinaryReader(fileStream))
                    {
                        return new
                        {
                            Loader = typeof(AudioAssetLoader).FullName,
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
