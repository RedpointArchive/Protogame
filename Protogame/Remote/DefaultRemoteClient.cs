using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Protogame
{
    public class DefaultRemoteClient : IRemoteClient
    {
        private readonly IRemoteClientInboundHandler[] _notifiers;
        private readonly TcpListener _inboundMessagesTcpListener;
        private readonly TcpListener _outboundMessagesTcpListener;
        private readonly Task _task;
        private BinaryReader _outboundReader;
        private BinaryWriter _outboundWriter;
        private SemaphoreSlim _outboundSemaphore;
        private TcpClient _outboundClient;
        private BinaryReader _inboundReader;
        private BinaryWriter _inboundWriter;

        public bool IsConnected => _outboundWriter != null;

        public DefaultRemoteClient(IRemoteClientInboundHandler[] notifiers)
        {
            _notifiers = notifiers;
            _inboundMessagesTcpListener = new TcpListener(IPAddress.Any, 23400);
            _inboundMessagesTcpListener.Start();
            _outboundMessagesTcpListener = new TcpListener(IPAddress.Any, 23401);
            _outboundMessagesTcpListener.Start();

            _outboundSemaphore = new SemaphoreSlim(1);

            _task = Task.Run(() => ListenForClients());
        }

        private async Task ListenForClients()
        {
            while (true)
            {
                TcpClient inboundClient = null;
                try
                {
                    inboundClient = await _inboundMessagesTcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                    _outboundClient = await _outboundMessagesTcpListener.AcceptTcpClientAsync().ConfigureAwait(false);

                    var inboundStream = inboundClient.GetStream();
                    var outboundStream = _outboundClient.GetStream();

                    _inboundReader = new BinaryReader(inboundStream);
                    _inboundWriter = new BinaryWriter(inboundStream);

                    _outboundReader = new BinaryReader(outboundStream);
                    _outboundWriter = new BinaryWriter(outboundStream);

                    foreach (var notifier in _notifiers)
                    {
                        await notifier.OnConnected(this).ConfigureAwait(false);
                    }

                    while (inboundClient.Connected)
                    {
                        var request = _inboundReader.ReadString();
                        foreach (var notifier in _notifiers)
                        {
                            if (await notifier.OnInboundMessage(this, request).ConfigureAwait(false))
                            {
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

        public async Task WaitUntilConnected()
        {
            while (_outboundWriter == null)
            {
                await Task.Yield();
            }
        }

        public async Task<IRemoteClientReaderWriter> ObtainOutboundReaderWriter()
        {
            await _outboundSemaphore.WaitAsync().ConfigureAwait(false);

            return new DefaultRemoteClientReaderWriter(_outboundReader, _outboundWriter);
        }

        public IRemoteClientReaderWriter TryObtainOutboundReaderWriter()
        {
            if (_outboundSemaphore.Wait(0))
            {
                return new DefaultRemoteClientReaderWriter(_outboundReader, _outboundWriter);
            }

            return null;
        }

        public async Task<IRemoteClientReaderWriter> ObtainInboundReaderWriter()
        {
            return new DefaultRemoteClientReaderWriter(_inboundReader, _inboundWriter);
        }

        public void ReleaseOutboundReaderWriter()
        {
            _outboundSemaphore.Release();
        }

        private class DefaultRemoteClientReaderWriter : IRemoteClientReaderWriter
        {
            private BinaryReader _outboundReader;
            private BinaryWriter _outboundWriter;

            public DefaultRemoteClientReaderWriter(BinaryReader outboundReader, BinaryWriter outboundWriter)
            {
                _outboundReader = outboundReader;
                _outboundWriter = outboundWriter;
            }

            public BinaryReader Reader => _outboundReader;

            public BinaryWriter Writer => _outboundWriter;
        }
    }
}