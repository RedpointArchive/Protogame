using System;
using System.IO;
using System.Threading.Tasks;

namespace Protogame
{
    public class SourceDirLocalFilesystemAssetFsLayer : IAssetFsLayer
    {
        private readonly LocalFilesystemAssetFsLayer _localLayer;

        public SourceDirLocalFilesystemAssetFsLayer()
        {
            _localLayer = null;

            var sourcePath = Path.Combine(Environment.CurrentDirectory, "Content", ".source");
            if (!File.Exists(sourcePath))
            {
                return;
            }

            using (var reader = new StreamReader(sourcePath))
            {
                sourcePath = reader.ReadLine();

                if (string.Equals(sourcePath, Path.Combine(Environment.CurrentDirectory, "Content"), StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            Directory.CreateDirectory(sourcePath);
            _localLayer = new LocalFilesystemAssetFsLayer(sourcePath);
        }

        public async Task<IAssetFsFile> Get(string name)
        {
            if (_localLayer != null)
            {
                return await _localLayer.Get(name);
            }

            return null;
        }
        
        public async Task<IAssetFsFile[]> List()
        {
            if (_localLayer != null)
            {
                return await _localLayer.List();
            }

            return new IAssetFsFile[0];
        }

        public void RegisterUpdateNotifier(Func<string, Task> onAssetUpdated)
        {
            _localLayer.RegisterUpdateNotifier(onAssetUpdated);
        }

        public void UnregisterUpdateNotifier(Func<string, Task> onAssetUpdated)
        {
            _localLayer.UnregisterUpdateNotifier(onAssetUpdated);
        }
    }
}
