using Protoinject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Protogame
{
    public class RemoteClientAssetFs : IAssetFsLayer, IDisposable
    {
        private readonly TcpListener _inboundMessagesTcpListener;
        private readonly TcpListener _outboundMessagesTcpListener;
        private readonly Task _task;
        private BinaryReader _outboundReader;
        private BinaryWriter _outboundWriter;
        private SemaphoreSlim _outboundSemaphore;
        private TcpClient _outboundClient;
        private readonly Dictionary<string, IAssetFsFile> _cachedFiles;
        private readonly HashSet<Func<string, Task>> _onAssetUpdated;

        public RemoteClientAssetFs()
        {
            _cachedFiles = new Dictionary<string, IAssetFsFile>();
            _onAssetUpdated = new HashSet<Func<string, Task>>();

            _inboundMessagesTcpListener = new TcpListener(IPAddress.Any, 23400);
            _inboundMessagesTcpListener.Start();
            _outboundMessagesTcpListener = new TcpListener(IPAddress.Any, 23401);
            _outboundMessagesTcpListener.Start();

            _outboundSemaphore = new SemaphoreSlim(1);

            _task = Task.Run(() => ListenForClients());
        }

        public async Task ListenForClients()
        {
            while (true)
            {
                TcpClient inboundClient = null;
                try
                {
                    inboundClient = await _inboundMessagesTcpListener.AcceptTcpClientAsync();
                    _outboundClient = await _outboundMessagesTcpListener.AcceptTcpClientAsync();

                    var inboundStream = inboundClient.GetStream();
                    var outboundStream = _outboundClient.GetStream();

                    var inboundReader = new BinaryReader(inboundStream);
                    var inboundWriter = new BinaryWriter(inboundStream);

                    _outboundReader = new BinaryReader(outboundStream);
                    _outboundWriter = new BinaryWriter(outboundStream);

                    await _outboundSemaphore.WaitAsync();
                    try
                    {
                        _outboundWriter.Write("list-assets");
                        _outboundWriter.Flush();
                        var assetCount = _outboundReader.ReadInt32();
                        var assets = new RemoteAssetFsFile[assetCount];
                        for (var i = 0; i < assetCount; i++)
                        {
                            assets[i] = ReadRemoteAssetFsFileFromStream(_outboundReader);
                        }
                        var assetsByName = assets.ToDictionary(k => k.Name, v => v);
                        foreach (var kv in assetsByName)
                        {
                            _cachedFiles[kv.Key] = kv.Value;
                            NotifyChanged(kv.Key);
                        }
                        foreach (var name in _cachedFiles.Keys.ToArray())
                        {
                            if (!assetsByName.ContainsKey(name))
                            {
                                _cachedFiles.Remove(name);
                                NotifyChanged(name);
                            }
                        }
                    }
                    finally
                    {
                        _outboundSemaphore.Release();
                    }

                    while (inboundClient.Connected)
                    {
                        var request = inboundReader.ReadString();
                        switch (request)
                        {
                            case "updated-asset":
                                {
                                    var asset = ReadRemoteAssetFsFileFromStream(inboundReader);
                                    _cachedFiles[asset.Name] = asset;
                                    NotifyChanged(asset.Name);
                                    break;
                                }
                            case "deleted-asset":
                                {
                                    var assetName = inboundReader.ReadString();
                                    _cachedFiles.Remove(assetName);
                                    NotifyChanged(assetName);
                                    break;
                                }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    inboundClient?.Close();
                    _outboundClient?.Close();
                }
            }
        }
        
        public void Dispose()
        {
            try
            {
                _task.Dispose();
            }
            catch { }
            try
            {
                _inboundMessagesTcpListener.Stop();
            }
            catch { }
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

        public void UnregisterUpdateNotifier(Func<string, Task> onAssetUpdated)
        {
            if (_onAssetUpdated.Contains(onAssetUpdated))
            {
                _onAssetUpdated.Remove(onAssetUpdated);
            }
        }

        private async Task WaitUntilConnected()
        {
            while (_outboundWriter == null)
            {
                await Task.Yield();
            }
        }

        private RemoteAssetFsFile ReadRemoteAssetFsFileFromStream(BinaryReader reader)
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
            return new RemoteAssetFsFile(this, name, extension, modificationUtcTimestampTicks, dependencies);
        }

        private class RemoteAssetFsFile : IAssetFsFile
        {
            private readonly RemoteClientAssetFs _remoteClientAssetFs;
            private readonly string[] _dependencies;
            private byte[] _cachedContent;

            public RemoteAssetFsFile(RemoteClientAssetFs remoteClientAssetFs, string name, string extension, long modificationUtcTimestampTicks, string[] dependencies)
            {
                _remoteClientAssetFs = remoteClientAssetFs;
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

                await _remoteClientAssetFs.WaitUntilConnected();
                await _remoteClientAssetFs._outboundSemaphore.WaitAsync();
                try
                {
                    _remoteClientAssetFs._outboundWriter.Write("get-asset-content");
                    _remoteClientAssetFs._outboundWriter.Write(Name);
                    _remoteClientAssetFs._outboundWriter.Flush();
                    if (_remoteClientAssetFs._outboundReader.ReadBoolean())
                    {
                        var contentLength = _remoteClientAssetFs._outboundReader.ReadInt32();
                        _cachedContent = _remoteClientAssetFs._outboundReader.ReadBytes(contentLength);
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
                    _remoteClientAssetFs._outboundSemaphore.Release();
                }
            }

            public async Task<string[]> GetDependentOnAssetFsFileNames()
            {
                return _dependencies;
            }
        }
    }
}