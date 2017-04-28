using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Protogame
{
    public class EmbeddedAssetFsLayer : IAssetFsLayer
    {
        private readonly Dictionary<string, EmbeddedAssetFsFile> _knownAssets;

        protected EmbeddedAssetFsLayer(bool isSource)
        {
            Func<string, bool> filter;
            if (isSource)
            {
                filter = (resource) => !resource.EndsWith("-" + TargetPlatformUtility.GetExecutingPlatform() + ".bin");
            }
            else
            {
                filter = (resource) => resource.EndsWith("-" + TargetPlatformUtility.GetExecutingPlatform() + ".bin");
            }

            var assets = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where !assembly.IsDynamic
                          where assembly.GetName().Name != "mscorlib" && assembly.GetName().Name != "System" && 
                          !assembly.GetName().Name.StartsWith("System.")
                          from resource in assembly.GetManifestResourceNames()
                          where resource.StartsWith(assembly.GetName().Name) && filter(resource)
                          let name = EmbeddedNameToAssetName(assembly.GetName().Name, resource)
                          where !string.IsNullOrWhiteSpace(name)
                          select new EmbeddedAssetFsFile(name, assembly, resource))
                 .ToList();
            _knownAssets = new Dictionary<string, EmbeddedAssetFsFile>();
            foreach (var asset in assets)
            {
                _knownAssets[asset.Name] = asset;
            }
        }

        private static string EmbeddedNameToAssetName(string assemblyName, string resourceName)
        {
            if (resourceName.StartsWith(assemblyName + "."))
            {
                resourceName = resourceName.Substring(assemblyName.Length + 1);
            }

            if (resourceName.StartsWith("Resources."))
            {
                resourceName = resourceName.Substring("Resources.".Length);
            }

            if (!resourceName.Contains("."))
            {
                return null;
            }

            // Strip off the file extension.
            resourceName = resourceName.Substring(0, resourceName.LastIndexOf('.'));

            if (resourceName.EndsWith("-" + TargetPlatformUtility.GetExecutingPlatform()))
            {
                resourceName = resourceName.Substring(0, resourceName.Length - (TargetPlatformUtility.GetExecutingPlatform().ToString().Length + 1));
            }

            return resourceName;
        }

        public async Task<IAssetFsFile> Get(string name)
        {
            if (_knownAssets.ContainsKey(name))
            {
                return _knownAssets[name];
            }

            return null;
        }

        public void GetChangedSinceLastUpdate(ref List<string> names)
        {
            // Never updates.
        }

        public async Task<IAssetFsFile[]> List()
        {
            return _knownAssets.Values.ToArray();
        }

        private class EmbeddedAssetFsFile : IAssetFsFile
        {
            private readonly Assembly _assembly;
            private readonly string _resource;

            public EmbeddedAssetFsFile(string name, Assembly assembly, string resource)
            {
                _assembly = assembly;
                _resource = resource;

                Name = name;
                Extension = resource.Substring(resource.LastIndexOf('.') + 1);
                ModificationTimeUtcTimestamp = DateTimeOffset.MinValue;

                if (Extension == "bin")
                {
                    // Make sure embedded compiled assets rank slightly higher
                    // than embedded source assets.
                    ModificationTimeUtcTimestamp = ModificationTimeUtcTimestamp.AddSeconds(1);
                }
            }

            public string Name { get; }

            public string Extension { get; }

            public DateTimeOffset ModificationTimeUtcTimestamp { get; }

            public async Task<Stream> GetContentStream()
            {
                var memory = new MemoryStream();
                using (var stream = _assembly.GetManifestResourceStream(_resource))
                {
                    await stream.CopyToAsync(memory).ConfigureAwait(false);
                }
                memory.Seek(0, SeekOrigin.Begin);
                return memory;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
