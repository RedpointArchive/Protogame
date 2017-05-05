#if PLATFORM_ANDROID

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Protogame
{
    public class AndroidAssetFsLayer : IAssetFsLayer
    {
        private readonly string _basePath;
        private readonly Dictionary<string, IAssetFsFile> _knownFiles;

        public AndroidAssetFsLayer(string basePath)
        {
            _basePath = basePath;
            _knownFiles = new Dictionary<string, IAssetFsFile>();

            ScanResources(_basePath);
        }

        protected bool AcceptAsset(string assetName)
        {
            if (string.IsNullOrWhiteSpace(assetName))
            {
                return false;
            }

            if (Enum.GetNames(typeof(TargetPlatform)).Any(x => assetName.StartsWith(x)))
            {
                return false;
            }

            return true;
        }

        private bool ScanResources(string path)
        {
            var entries = global::Android.App.Application.Context.Assets.List(path);
            if (entries.Length == 0)
            {
                return false;
            }
            foreach (var entry in entries)
            {
                if (!ScanResources((path + "/" + entry).TrimStart('/')))
                {
                    var name = ResourceToAssetName((path.Substring(_basePath.Length) + "/" + entry).TrimStart('/'));
                    if (AcceptAsset(name) && !_knownFiles.ContainsKey(name))
                    {
                        _knownFiles.Add(name, new AndroidAssetFsFile(name, (path + "/" + entry).TrimStart('/')));
                    }
                }
            }
            return true;
        }

        private string ResourceToAssetName(string resourcePath)
        {
            return resourcePath.Replace('/', '.').Substring(0, resourcePath.LastIndexOf('.'));
        }

        public async Task<IAssetFsFile> Get(string name)
        {
            if (_knownFiles.ContainsKey(name))
            {
                return _knownFiles[name];
            }

            return null;
        }

        public async Task<IAssetFsFile[]> List()
        {
            return _knownFiles.Values.ToArray();
        }

        public void RegisterUpdateNotifier(Func<string, Task> onAssetUpdated)
        {
        }

        public void UnregisterUpdateNotifier(Func<string, Task> onAssetUpdated)
        {
        }

        private class AndroidAssetFsFile : IAssetFsFile
        {
            private readonly string _resourcePath;

            public AndroidAssetFsFile(string assetName, string resourcePath)
            {
                _resourcePath = resourcePath;
                Extension = _resourcePath.Substring(_resourcePath.LastIndexOf('.') + 1);
                Name = assetName;

                if (Extension == "bin")
                {
                    ModificationTimeUtcTimestamp = DateTimeOffset.UtcNow;
                }
                else
                {
                    ModificationTimeUtcTimestamp = DateTimeOffset.UtcNow.AddHours(-1);
                }
            }

            public DateTimeOffset ModificationTimeUtcTimestamp { get; }

            public string Name { get; }

            public string Extension { get; }

            public async Task<Stream> GetContentStream()
            {
                var stream = global::Android.App.Application.Context.Assets.Open(_resourcePath);
                var memory = new MemoryStream();
                await stream.CopyToAsync(memory).ConfigureAwait(false);
                memory.Seek(0, SeekOrigin.Begin);
                return memory;
            }

            public Task<string[]> GetDependentOnAssetFsFileNames()
            {
                return Task.FromResult(new string[0]);
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}

#endif