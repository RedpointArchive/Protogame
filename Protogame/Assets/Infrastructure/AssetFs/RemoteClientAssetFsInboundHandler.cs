using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Protogame
{
    public class RemoteClientAssetFsInboundHandler : IRemoteClientInboundHandler
    {
        private readonly RemoteClientAssetFsLayer _remoteClientAssetFsLayer;

        public RemoteClientAssetFsInboundHandler(
            IAssetFsLayer[] assetFsLayers)
        {
            _remoteClientAssetFsLayer = assetFsLayers.OfType<RemoteClientAssetFsLayer>().FirstOrDefault();
        }

        public async Task OnConnected(IRemoteClient remoteClient)
        {
            if (_remoteClientAssetFsLayer == null)
            {
                return;
            }

            var outbound = await remoteClient.ObtainOutboundReaderWriter().ConfigureAwait(false);
            
            try
            {
                outbound.Writer.Write("list-assets");
                outbound.Writer.Flush();
                var assetCount = outbound.Reader.ReadInt32();
                var assets = new RemoteAssetFsFile[assetCount];
                for (var i = 0; i < assetCount; i++)
                {
                    assets[i] = ReadRemoteAssetFsFileFromStream(remoteClient, outbound.Reader);
                }
                var assetsByName = assets.ToDictionary(k => k.Name, v => v);
                foreach (var kv in assetsByName)
                {
                    _remoteClientAssetFsLayer.SetCachedFile(kv.Key, kv.Value);
                }
                foreach (var name in _remoteClientAssetFsLayer.GetCachedFileKeys())
                {
                    if (!assetsByName.ContainsKey(name))
                    {
                        _remoteClientAssetFsLayer.RemoveCachedFile(name);
                    }
                }
            }
            finally
            {
                remoteClient.ReleaseOutboundReaderWriter();
            }
        }

        public async Task<bool> OnInboundMessage(IRemoteClient remoteClient, string request)
        {
            if (_remoteClientAssetFsLayer == null)
            {
                return false;
            }

            var inbound = await remoteClient.ObtainInboundReaderWriter().ConfigureAwait(false);

            switch (request)
            {
                case "updated-asset":
                    {
                        var asset = ReadRemoteAssetFsFileFromStream(remoteClient, inbound.Reader);
                        _remoteClientAssetFsLayer.SetCachedFile(asset.Name, asset);
                        break;
                    }
                case "deleted-asset":
                    {
                        var assetName = inbound.Reader.ReadString();
                        _remoteClientAssetFsLayer.RemoveCachedFile(assetName);
                        break;
                    }
            }

            return false;
        }

        private RemoteAssetFsFile ReadRemoteAssetFsFileFromStream(IRemoteClient remoteClient, BinaryReader reader)
        {
            var name = reader.ReadString();
            var extension = reader.ReadString();
            var modificationUtcTimestampTicks = reader.ReadInt64();
            var dependentCount = reader.ReadInt32();
            var dependencies = new string[dependentCount];
            for (var i = 0; i < dependentCount; i++)
            {
                dependencies[i] = reader.ReadString();
            }
            return new RemoteAssetFsFile(remoteClient, name, extension, modificationUtcTimestampTicks, dependencies);
        }

        public class RemoteAssetFsFile : IAssetFsFile
        {
            private readonly IRemoteClient _remoteClient;
            private readonly string[] _dependencies;
            private byte[] _cachedContent;

            public RemoteAssetFsFile(IRemoteClient remoteClient, string name, string extension, long modificationUtcTimestampTicks, string[] dependencies)
            {
                _remoteClient = remoteClient;
                _dependencies = dependencies;
                Name = name;
                Extension = extension;
                ModificationTimeUtcTimestamp = new DateTimeOffset(modificationUtcTimestampTicks, TimeSpan.Zero);
            }

            public string Name { get; }

            public string Extension { get; }

            public DateTimeOffset ModificationTimeUtcTimestamp { get; }

            public async Task<Stream> GetContentStream()
            {
                if (_cachedContent != null)
                {
                    return new MemoryStream(_cachedContent);
                }

                await _remoteClient.WaitUntilConnected().ConfigureAwait(false);
                var outbound = await _remoteClient.ObtainOutboundReaderWriter().ConfigureAwait(false);
                try
                {
                    outbound.Writer.Write("get-asset-content");
                    outbound.Writer.Write(Name);
                    outbound.Writer.Flush();
                    if (outbound.Reader.ReadBoolean())
                    {
                        var contentLength = outbound.Reader.ReadInt32();
                        _cachedContent = outbound.Reader.ReadBytes(contentLength);
                        return new MemoryStream(_cachedContent);
                    }
                    else
                    {
                        // TODO: Server was unable to provide content!
                        return new MemoryStream();
                    }
                }
                finally
                {
                    _remoteClient.ReleaseOutboundReaderWriter();
                }
            }

            public async Task<string[]> GetDependentOnAssetFsFileNames()
            {
                return _dependencies;
            }
        }
    }
}