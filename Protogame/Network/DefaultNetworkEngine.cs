using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Protogame
{
    public class DefaultNetworkEngine : INetworkEngine
    {
        private readonly INetworkFactory _networkFactory;
        private readonly IProfiler _profiler;
        private readonly INetworkMessageSerialization _networkMessageSerialization;
        private IWorld _currentWorld;
        private IServerWorld _currentServerWorld;
        private NetworkShadowWorld _shadowWorld;

        private readonly Dictionary<IWorld, MxDispatcher> _dispatchers;
        private readonly Dictionary<IServerWorld, MxDispatcher> _serverDispatchers;
        private readonly Dictionary<IWorld, bool> _dispatcherChanged;
        private readonly Dictionary<IServerWorld, bool> _serverDispatcherChanged;

        private readonly MxDispatcher[] _currentDispatchers;
        private static readonly MxDispatcher[] _emptyDispatchers = new MxDispatcher[0];

        private readonly Dictionary<int, WeakReference> _objectReferences;
        private int _clientRenderDelayTicks;

        private List<INetworkFrame> _recentNetworkFrames;
        private DefaultNetworkFrame _currentNetworkFrame;

        private int _networkFrameId;

        public DefaultNetworkEngine(
            INetworkFactory networkFactory,
            IProfiler profiler,
            INetworkMessageSerialization networkMessageSerialization)
        {
            _networkFactory = networkFactory;
            _profiler = profiler;
            _networkMessageSerialization = networkMessageSerialization;
            _dispatchers = new Dictionary<IWorld, MxDispatcher>();
            _serverDispatchers = new Dictionary<IServerWorld, MxDispatcher>();
            _dispatcherChanged = new Dictionary<IWorld, bool>();
            _serverDispatcherChanged = new Dictionary<IServerWorld, bool>();
            _currentDispatchers = new MxDispatcher[1];
            _objectReferences = new Dictionary<int, WeakReference>();
            _recentNetworkFrames = new List<INetworkFrame>();
            _currentNetworkFrame = new DefaultNetworkFrame(_networkFrameId++);

            _clientRenderDelayTicks = -200;
        }

        public void AttachDispatcher(IWorld world, MxDispatcher dispatcher)
        {
            _dispatchers[world] = dispatcher;
            _dispatcherChanged[world] = true;
        }

        public void AttachDispatcher(IServerWorld world, MxDispatcher dispatcher)
        {
            _serverDispatchers[world] = dispatcher;
            _serverDispatcherChanged[world] = true;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (gameContext.World == null)
            {
                return;
            }

            UpdateFrames();

            if (gameContext.World != _currentWorld || _shadowWorld == null)
            {
                if (_shadowWorld != null)
                {
                    _shadowWorld.Dispose();
                }

                _shadowWorld = _networkFactory.CreateShadowWorld();
                _currentWorld = gameContext.World;

                _shadowWorld.OnMessageRecievedStatisticsAction += OnMessageRecievedStatisticsAction;
            }

            if (_dispatcherChanged.ContainsKey(gameContext.World) &&
                _dispatcherChanged[gameContext.World])
            {
                _shadowWorld.Dispatcher = _dispatchers[gameContext.World];
                _dispatcherChanged[gameContext.World] = false;
                _currentDispatchers[0] = _shadowWorld.Dispatcher;
            }

            using (_profiler.Measure("net-step"))
            {
                _shadowWorld.Update(gameContext, updateContext);
            }
        }

        private void UpdateFrames()
        {
            if (_currentNetworkFrame != null)
            {
                _currentNetworkFrame.Calculate();
                _recentNetworkFrames.Add(_currentNetworkFrame);
            }

            if (_recentNetworkFrames.Count > 120)
            {
                _recentNetworkFrames.RemoveAt(0);
            }

            _currentNetworkFrame = new DefaultNetworkFrame(_networkFrameId++);

        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            UpdateFrames();

            if (serverContext.World != _currentServerWorld || _shadowWorld == null)
            {
                if (_shadowWorld != null)
                {
                    _shadowWorld.Dispose();
                }

                _shadowWorld = _networkFactory.CreateShadowWorld();
                _currentServerWorld = serverContext.World;

                _shadowWorld.OnMessageRecievedStatisticsAction += OnMessageRecievedStatisticsAction;
            }

            if (_serverDispatcherChanged.ContainsKey(serverContext.World) &&
                _serverDispatcherChanged[serverContext.World])
            {
                _shadowWorld.Dispatcher = _serverDispatchers[serverContext.World];
                _serverDispatcherChanged[serverContext.World] = false;
                _currentDispatchers[0] = _shadowWorld.Dispatcher;
            }

            using (_profiler.Measure("net-step"))
            {
                _shadowWorld.Update(serverContext, updateContext);
            }
        }

        private void OnMessageRecievedStatisticsAction(byte[] bytes)
        {
            var message = _networkMessageSerialization.Deserialize(bytes);
            if (message == null)
            {
                return;
            }

            var type = message.GetType();
            if (!_currentNetworkFrame.BytesReceivedByMessageType.ContainsKey(type))
            {
                _currentNetworkFrame.BytesReceivedByMessageType[type] = 0;
                _currentNetworkFrame.MessagesReceivedByMessageType[type] = 0;
            }

            _currentNetworkFrame.BytesReceivedByMessageType[type] += bytes.Length;
            _currentNetworkFrame.MessagesReceivedByMessageType[type]++;
        }

        public MxDispatcher[] CurrentDispatchers
        {
            get
            {
                if (_currentDispatchers[0] == null)
                {
                    return _emptyDispatchers;
                }
                else
                {
                    return _currentDispatchers;
                }
            }
        }

        public IEnumerable<KeyValuePair<int, object>> ListObjectsByNetworkId()
        {
            return _objectReferences.Where(v => v.Value.Target != null)
                .Select(x => new KeyValuePair<int, object>(x.Key, x.Value.Target));
        }

        public T FindObjectByNetworkId<T>(int id)
        {
            if (!_objectReferences.ContainsKey(id))
            {
                return default(T);
            }

            return (T)(_objectReferences[id].Target);
        }

        public object FindObjectByNetworkId(int id)
        {
            if (!_objectReferences.ContainsKey(id))
            {
                return null;
            }

            return _objectReferences[id].Target;
        }

        public void RegisterObjectAsNetworkId(int id, object obj)
        {
            _objectReferences[id] = new WeakReference(obj);
        }

        public void DeregisterObjectFromNetworkId(int id)
        {
            _objectReferences.Remove(id);
        }

        public int ClientRenderDelayTicks
        {
            get { return _clientRenderDelayTicks; }
            set
            {
                if (value > 0)
                {
                    throw new ArgumentException("This value must be either 0, or a negative number.", nameof(value));
                }

                _clientRenderDelayTicks = value;
            }
        }

        public void Send<T>(MxDispatcher dispatcher, MxClientGroup target, T message, bool reliable = false)
        {
            if (target.RealtimeClients.Count == 0)
            {
                throw new InvalidOperationException(
                    "Attempted to send message to group " + target +
                    ", but it has no clients.");
            }

            var serialized = _networkMessageSerialization.Serialize(message);
            dispatcher.Send(target, serialized, reliable);

            var type = typeof(T);
            if (!_currentNetworkFrame.BytesSentByMessageType.ContainsKey(type))
            {
                _currentNetworkFrame.BytesSentByMessageType[type] = 0;
                _currentNetworkFrame.MessagesSentByMessageType[type] = 0;
            }

            _currentNetworkFrame.BytesSentByMessageType[type] += serialized.Length;
            _currentNetworkFrame.MessagesSentByMessageType[type]++;
        }

        public IEnumerable<INetworkFrame> GetRecentFrames()
        {
            return _recentNetworkFrames.AsReadOnly();
        }

        public void LogSynchronisationEvent(string log)
        {
            _currentNetworkFrame.SynchronisationLog.Add(log);
        }
    }
}
