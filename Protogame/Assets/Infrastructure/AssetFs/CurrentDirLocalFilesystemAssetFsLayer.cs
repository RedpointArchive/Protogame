using System;
using System.IO;

namespace Protogame
{
    public class CurrentDirLocalFilesystemAssetFsLayer : LocalFilesystemAssetFsLayer
    {
        public CurrentDirLocalFilesystemAssetFsLayer(IBaseDirectory baseDirectory)
            : base(GetPath(baseDirectory))
        { }

        private static string GetPath(IBaseDirectory baseDirectory)
        {
            var path = Path.Combine(baseDirectory.FullPath, "Content");
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
