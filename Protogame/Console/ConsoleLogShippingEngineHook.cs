using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Protogame
{
    public class ConsoleLogShippingEngineHook : IEngineHook
    {
        private Task _coroutineInstance;
        private readonly IRemoteClient _remoteClient;
        private readonly ICoroutine _coroutine;
        private readonly ILogShipping _logShipping;

        public ConsoleLogShippingEngineHook(
            IRemoteClient remoteClient,
            ICoroutine coroutine,
            ILogShipping logShipping)
        {
            _remoteClient = remoteClient;
            _coroutine = coroutine;
            _logShipping = logShipping;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (_coroutineInstance == null || _coroutineInstance.IsCompleted)
            {
                _coroutineInstance = _coroutine.Run(RunLogShipping);
            }
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            if (_coroutineInstance == null || _coroutineInstance.IsCompleted)
            {
                _coroutineInstance = _coroutine.Run(RunLogShipping);
            }
        }

        public async Task RunLogShipping()
        {
            while (true)
            {
                await Task.Yield();

                if (!_remoteClient.IsConnected)
                {
                    continue;
                }

                var outbound = _remoteClient.TryObtainOutboundReaderWriter();
                if (outbound == null)
                {
                    continue;
                }
                try
                {
                    var logs = _logShipping.GetAndFlushLogs();
                    outbound.Writer.Write("logs");
                    outbound.Writer.Write((Int32)logs.Length);
                    for (var i = 0; i < logs.Length; i++)
                    {
                        outbound.Writer.Write((Int32)logs[i].LogLevel);
                        outbound.Writer.Write(logs[i].Message);
                    }
                    outbound.Writer.Flush();
                }
                finally
                {
                    _remoteClient.ReleaseOutboundReaderWriter();
                }
            }
        }
    }
}