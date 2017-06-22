using System;
#if PLATFORM_MACOS
using System.IO;
#endif

namespace Protogame
{
    public class DefaultBaseDirectory : IBaseDirectory
    {
        public DefaultBaseDirectory()
        {
#if PLATFORM_MACOS
            var assemblyDirectory = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            var resourcesDirectory = Path.Combine(assemblyDirectory, "..", "Resources");
            if (Directory.Exists(Path.Combine(resourcesDirectory, "Content")))
            {
                FullPath = new DirectoryInfo(resourcesDirectory).FullName;
            }
            else if (Directory.Exists(assemblyDirectory))
            {
                FullPath = new DirectoryInfo(assemblyDirectory).FullName;
            }
            else
            {
                FullPath = Environment.CurrentDirectory;
            }
#else
            FullPath = Environment.CurrentDirectory;
#endif
        }

        public string FullPath { get; }
    }
}
