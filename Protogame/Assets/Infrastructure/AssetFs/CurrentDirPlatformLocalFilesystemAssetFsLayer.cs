using System;
using System.IO;

namespace Protogame
{
    public class CurrentDirPlatformLocalFilesystemAssetFsLayer : LocalFilesystemAssetFsLayer
    {
        public CurrentDirPlatformLocalFilesystemAssetFsLayer(IBaseDirectory baseDirectory)
            : base(GetPath(baseDirectory))
        { }

        private static string GetPath(IBaseDirectory baseDirectory)
        {
            var path = Path.Combine(baseDirectory.FullPath, "Content", TargetPlatformUtility.GetExecutingPlatform().ToString());
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
