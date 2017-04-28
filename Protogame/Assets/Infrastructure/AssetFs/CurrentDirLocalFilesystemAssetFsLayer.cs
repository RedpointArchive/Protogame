using System;
using System.IO;

namespace Protogame
{
    public class CurrentDirLocalFilesystemAssetFsLayer : LocalFilesystemAssetFsLayer
    {
        public CurrentDirLocalFilesystemAssetFsLayer()
            : base(GetPath())
        { }

        private static string GetPath()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Content");
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
