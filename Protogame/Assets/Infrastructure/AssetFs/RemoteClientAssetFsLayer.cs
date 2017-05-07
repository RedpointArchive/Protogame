using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Protogame
{
    public class RemoteClientAssetFsLayer : IAssetFsLayer
    {
        private readonly Dictionary<string, IAssetFsFile> _cachedFiles;
        private readonly HashSet<Func<string, Task>> _onAssetUpdated;

        public RemoteClientAssetFsLayer()
        {
            _cachedFiles = new Dictionary<string, IAssetFsFile>();
            _onAssetUpdated = new HashSet<Func<string, Task>>();
        }

        public async Task<IAssetFsFile[]> List()
        {
            return _cachedFiles.Values.ToArray();
        }

        public async Task<IAssetFsFile> Get(string name)
        {
            if (_cachedFiles.ContainsKey(name))
            {
                return _cachedFiles[name];
            }

            return null;
        }

        private void NotifyChanged(string assetName)
        {
            foreach (var handler in _onAssetUpdated)
            {
                Task.Run(async () => await handler(assetName).ConfigureAwait(false));
            }
        }

        public void RegisterUpdateNotifier(Func<string, Task> onAssetUpdated)
        {
            if (!_onAssetUpdated.Contains(onAssetUpdated))
            {
                _onAssetUpdated.Add(onAssetUpdated);
            }
        }

        public void SetCachedFile(string key, RemoteClientAssetFsInboundHandler.RemoteAssetFsFile value)
        {
            _cachedFiles[key] = value;
            NotifyChanged(key);
        }

        public string[] GetCachedFileKeys()
        {
            return _cachedFiles.Keys.ToArray();
        }

        public void RemoveCachedFile(string key)
        {
            _cachedFiles.Remove(key);
            NotifyChanged(key);
        }

        public void UnregisterUpdateNotifier(Func<string, Task> onAssetUpdated)
        {
            if (_onAssetUpdated.Contains(onAssetUpdated))
            {
                _onAssetUpdated.Remove(onAssetUpdated);
            }
        }
    }
}