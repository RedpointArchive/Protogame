using System.IO;

namespace Protogame
{
    public class RawTextureLoadStrategy : ILoadStrategy
    {
        public bool ScanSourcePath { get { return false; } }

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
                        return new
                        {
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
}
