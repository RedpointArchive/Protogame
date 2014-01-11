using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                // If there are name-anim.fbx files, read those in as additional animations.
                var directory = file.Directory;
                var otherAnimations = new Dictionary<string, byte[]>();
                if (directory != null)
                {
                    var lastComponent = name.Split('.').Last();
                    var otherAnimationFiles = directory.GetFiles(lastComponent + "-*.fbx");
                    otherAnimations =
                        otherAnimationFiles.ToDictionary(
                            key => key.Name.Split('-').Last().Split('.').First(),
                            value => this.ReadModelData(value.FullName));
                }

                return new
                {
                    Loader = typeof(ModelAssetLoader).FullName,
                    PlatformData = (PlatformData)null,
                    RawData = this.ReadModelData(file.FullName),
                    RawAdditionalAnimations = otherAnimations,
                    SourcedFromRaw = true
                };
            }

            return null;
        }

        private byte[] ReadModelData(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var binary = new BinaryReader(fileStream))
                {
                    return binary.ReadBytes((int)fileStream.Length);
                }
            }
        }
    }
}
