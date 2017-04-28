using System;
using System.IO;

namespace Protogame
{
    public class CurrentDirPlatformLocalFilesystemAssetFsLayer : LocalFilesystemAssetFsLayer
    {
        public CurrentDirPlatformLocalFilesystemAssetFsLayer()
            : base(GetPath())
        { }

        private static string GetPath()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Content", TargetPlatformUtility.GetExecutingPlatform().ToString());
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
