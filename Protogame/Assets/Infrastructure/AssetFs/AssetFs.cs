﻿using Protoinject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Protogame
{
    public class AssetFs : IAssetFs, IAsynchronouslyConstructable, IDisposable
    {
        private readonly IAssetFsLayer[] _layers;
        private readonly Dictionary<string, IAssetFsFile> _knownAssets;
        private readonly HashSet<Func<string, Task>> _onAssetUpdated;
        private Task _updatingTask;

        public AssetFs(IAssetFsLayer[] layers)
        {
            _layers = layers;
            _knownAssets = new Dictionary<string, IAssetFsFile>();
            _onAssetUpdated = new HashSet<Func<string, Task>>();

            foreach (var layer in _layers)
            {
                layer.RegisterUpdateNotifier(OnAssetUpdated);
            }
        }

        private async Task OnAssetUpdated(string assetName)
        {
            // Wait until we've resorted assets before notifying our
            // consumers that an asset has been updated.
            await ProcessAsset(assetName).ConfigureAwait(false);

            foreach (var handler in _onAssetUpdated)
            {
                await handler(assetName).ConfigureAwait(false);
            }
        }

        public void RegisterUpdateNotifier(Func<string, Task> onAssetUpdated)
        {
            if (!_onAssetUpdated.Contains(onAssetUpdated))
            {
                _onAssetUpdated.Add(onAssetUpdated);
            }
        }

        public void UnregisterUpdateNotifier(Func<string, Task> onAssetUpdated)
        {
            if (_onAssetUpdated.Contains(onAssetUpdated))
            {
                _onAssetUpdated.Remove(onAssetUpdated);
            }
        }

        public async Task ConstructAsync()
        {
            var processedAssets = new HashSet<string>();
            foreach (var layer in _layers)
            {
                foreach (var asset in await layer.List().ConfigureAwait(false))
                {
                    if (!processedAssets.Contains(asset.Name))
                    {
                        await ProcessAsset(asset.Name).ConfigureAwait(false);
                        processedAssets.Add(asset.Name);
                    }
                }
            }
        }

        public async Task<IAssetFsFile> Get(string name)
        {
            if (_knownAssets.ContainsKey(name))
            {
                return _knownAssets[name];
            }

            return null;
        }

        public async Task<IAssetFsFile[]> List()
        {
            return _knownAssets.Values.ToArray();
        }

        private async Task ProcessAsset(string name)
        {
            var files = new List<IAssetFsFile>();
            foreach (var layer in _layers)
            {
                files.Add(await layer.Get(name).ConfigureAwait(false));
            }
            var file = files.Where(x => x != null).OrderByDescending(x => x.ModificationTimeUtcTimestamp).FirstOrDefault();
            if (file == null)
            {
                _knownAssets.Remove(name);
            }
            else
            {
                _knownAssets[name] = file;
            }
        }

        public void Dispose()
        {
            foreach (var layer in _layers)
            {
                layer.UnregisterUpdateNotifier(OnAssetUpdated);
            }
        }
    }
}
