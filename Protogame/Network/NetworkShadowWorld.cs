using System;
using System.Collections.Generic;

namespace Protogame
{
    public class NetworkShadowWorld : IDisposable
    {
        private readonly IEventEngine<INetworkEventContext> _networkEventEngine;

        private MxDispatcher _dispatcher;
        private readonly Queue<MxMessageEventArgs> _messagesRecieved;
        private readonly Queue<MxClientEventArgs> _clientsConnected;
        private readonly Queue<MxClientEventArgs> _clientsDisconnected;

        public NetworkShadowWorld(IEventEngine<INetworkEventContext> networkEventEngine)
        {
            _networkEventEngine = networkEventEngine;
            _messagesRecieved = new Queue<MxMessageEventArgs>();
            _clientsConnected = new Queue<MxClientEventArgs>();
            _clientsDisconnected = new Queue<MxClientEventArgs>();
        }

        public MxDispatcher Dispatcher
        {
            get { return _dispatcher; }
            set
            {
                if (_dispatcher != null)
                {
                    _dispatcher.MessageReceived -= DispatcherOnMessageReceived;
                    _dispatcher.ClientConnected -= DispatcherOnClientConnected;
                    _dispatcher.ClientDisconnectWarning -= DispatcherOnClientDisconnectWarning;
                    _dispatcher.ClientDisconnected -= DispatcherOnClientDisconnected;
                    _dispatcher.MessageAcknowledged -= DispatcherOnMessageAcknowledged;
                    _dispatcher.MessageLost -= DispatcherOnMessageLost;
                    _dispatcher.MessageSent -= DispatcherOnMessageSent;
                    _dispatcher.ReliableReceivedProgress -= DispatcherOnReliableReceivedProgress;
                    _dispatcher.ReliableSendProgress -= DispatcherOnReliableSendProgress;
                }

                _dispatcher = value;

                if (_dispatcher != null)
                {
                    _dispatcher.MessageReceived += DispatcherOnMessageReceived;
                    _dispatcher.ClientConnected += DispatcherOnClientConnected;
                    _dispatcher.ClientDisconnectWarning += DispatcherOnClientDisconnectWarning;
                    _dispatcher.ClientDisconnected += DispatcherOnClientDisconnected;
                    _dispatcher.MessageAcknowledged += DispatcherOnMessageAcknowledged;
                    _dispatcher.MessageLost += DispatcherOnMessageLost;
                    _dispatcher.MessageSent += DispatcherOnMessageSent;
                    _dispatcher.ReliableReceivedProgress += DispatcherOnReliableReceivedProgress;
                    _dispatcher.ReliableSendProgress += DispatcherOnReliableSendProgress;
                }
            }
        }

        private void DispatcherOnMessageReceived(object sender, MxMessageEventArgs mxMessageEventArgs)
        {
            _messagesRecieved.Enqueue(mxMessageEventArgs);
        }

        private void DispatcherOnClientConnected(object sender, MxClientEventArgs mxClientEventArgs)
        {
            _clientsConnected.Enqueue(mxClientEventArgs);
        }

        private void DispatcherOnClientDisconnectWarning(object sender, MxDisconnectEventArgs mxDisconnectEventArgs)
        {

        }

        private void DispatcherOnClientDisconnected(object sender, MxClientEventArgs mxClientEventArgs)
        {
            _clientsDisconnected.Enqueue(mxClientEventArgs);
        }

        private void DispatcherOnMessageAcknowledged(object sender, MxMessageEventArgs mxMessageEventArgs)
        {

        }

        private void DispatcherOnMessageLost(object sender, MxMessageEventArgs mxMessageEventArgs)
        {

        }

        private void DispatcherOnMessageSent(object sender, MxMessageEventArgs mxMessageEventArgs)
        {

        }

        private void DispatcherOnReliableReceivedProgress(object sender, MxReliabilityTransmitEventArgs mxReliabilityTransmitEventArgs)
        {

        }

        private void DispatcherOnReliableSendProgress(object sender, MxReliabilityTransmitEventArgs mxReliabilityTransmitEventArgs)
        {
            
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (Dispatcher == null)
            {
                return;
            }

            Dispatcher.Update();

            while (_messagesRecieved.Count > 0)
            {
                var @event = _messagesRecieved.Dequeue();

                _networkEventEngine.Fire(
                    new DefaultNetworkEventContext(),
                    new NetworkMessageReceivedEvent
                    {
                        Client = @event.Client,
                        Dispatcher = Dispatcher,
                        GameContext = gameContext,
                        ServerContext = null,
                        UpdateContext = updateContext,
                        Payload = @event.Payload,
                        ProtocolID = @event.ProtocolID
                    });
            }

            while (_clientsConnected.Count > 0)
            {
                var @event = _clientsConnected.Dequeue();

                _networkEventEngine.Fire(
                    new DefaultNetworkEventContext(),
                    new NetworkClientConnectedEvent
                    {
                        Client = @event.Client,
                        Dispatcher = Dispatcher,
                        GameContext = gameContext,
                        ServerContext = null,
                        UpdateContext = updateContext
                    });
            }

            while (_clientsDisconnected.Count > 0)
            {
                var @event = _clientsDisconnected.Dequeue();

                _networkEventEngine.Fire(
                    new DefaultNetworkEventContext(),
                    new NetworkClientDisconnectedEvent
                    {
                        Client = @event.Client,
                        Dispatcher = Dispatcher,
                        GameContext = gameContext,
                        ServerContext = null,
                        UpdateContext = updateContext
                    });
            }
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            if (Dispatcher == null)
            {
                return;
            }

            Dispatcher.Update();

            while (_messagesRecieved.Count > 0)
            {
                var @event = _messagesRecieved.Dequeue();

                _networkEventEngine.Fire(
                    new DefaultNetworkEventContext(),
                    new NetworkMessageReceivedEvent
                    {
                        Client = @event.Client,
                        Dispatcher = Dispatcher,
                        GameContext = null,
                        ServerContext = serverContext,
                        UpdateContext = updateContext,
                        Payload = @event.Payload,
                        ProtocolID = @event.ProtocolID
                    });
            }

            while (_clientsConnected.Count > 0)
            {
                var @event = _clientsConnected.Dequeue();

                _networkEventEngine.Fire(
                    new DefaultNetworkEventContext(),
                    new NetworkClientConnectedEvent
                    {
                        Client = @event.Client,
                        Dispatcher = Dispatcher,
                        GameContext = null,
                        ServerContext = serverContext,
                        UpdateContext = updateContext
                    });
            }

            while (_clientsDisconnected.Count > 0)
            {
                var @event = _clientsDisconnected.Dequeue();

                _networkEventEngine.Fire(
                    new DefaultNetworkEventContext(),
                    new NetworkClientDisconnectedEvent
                    {
                        Client = @event.Client,
                        Dispatcher = Dispatcher,
                        GameContext = null,
                        ServerContext = serverContext,
                        UpdateContext = updateContext
                    });
            }
        }

        public void Dispose()
        {

        }
    }
}
