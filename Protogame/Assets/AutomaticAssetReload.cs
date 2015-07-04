using System;
using Protogame;
using System.IO;
using System.Collections.Generic;

namespace Protogame
{
    public class AutomaticAssetReload : IAutomaticAssetReload
    {
        private readonly IAssetManager _assetManager;

        private readonly IRawAssetLoader _rawAssetLoader;

        private bool _started;

        public AutomaticAssetReload(
            IAssetManager assetManager,
            IRawAssetLoader rawAssetLoader)
        {
            _assetManager = assetManager;
            _rawAssetLoader = rawAssetLoader;
            _started = false;
        }

        public void Start()
        {
            if (_started)
            {
                return;
            }

            var assetNames = _assetManager.GetAllNames();
            var pathToAssetMapping = new Dictionary<string, string>();

            foreach (var name in assetNames)
            {
                var paths = _rawAssetLoader.GetPotentialPathsForRawAsset(name);

                foreach (var path in paths)
                {
                    pathToAssetMapping[path] = name;

                    var fi = new FileInfo(path);
                    if (!Directory.Exists(fi.Directory.FullName))
                    {
                        continue;
                    }

                    var watcher = new FileSystemWatcher();
                    watcher.Path = fi.Directory.FullName;
                    watcher.Filter = fi.Name;
                    watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
                    watcher.Changed += new FileSystemEventHandler((sender, e) => {
                        _assetManager.Dirty(name);
                    });
                    watcher.Created += new FileSystemEventHandler((sender, e) => {
                        _assetManager.Dirty(name);
                    });
                    watcher.Deleted += new FileSystemEventHandler((sender, e) => {
                        _assetManager.Dirty(name);
                    });
                    watcher.Renamed += new RenamedEventHandler((sender, e) => {
                        if (pathToAssetMapping.ContainsKey(e.FullPath))
                        {
                            _assetManager.Dirty(pathToAssetMapping[e.FullPath]);
                        }

                        _assetManager.Dirty(name);
                    });
                    watcher.EnableRaisingEvents = true;
                }
            }

            _started = true;
        }
    }
}

