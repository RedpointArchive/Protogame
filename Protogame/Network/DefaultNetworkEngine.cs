using System;
using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    public class DefaultNetworkEngine : INetworkEngine
    {
        private readonly INetworkFactory _networkFactory;
        private readonly IProfiler _profiler;
        private IWorld _currentWorld;
        private IServerWorld _currentServerWorld;
        private NetworkShadowWorld _shadowWorld;

        private readonly Dictionary<IWorld, MxDispatcher> _dispatchers;
        private readonly Dictionary<IServerWorld, MxDispatcher> _serverDispatchers;
        private readonly Dictionary<IWorld, bool> _dispatcherChanged;
        private readonly Dictionary<IServerWorld, bool> _serverDispatcherChanged;

        private readonly MxDispatcher[] _currentDispatchers;

        private readonly Dictionary<int, WeakReference> _objectReferences;
        private int _clientRenderDelayTicks;

        public DefaultNetworkEngine(
            INetworkFactory networkFactory,
            IProfiler profiler)
        {
            _networkFactory = networkFactory;
            _profiler = profiler;
            _dispatchers = new Dictionary<IWorld, MxDispatcher>();
            _serverDispatchers = new Dictionary<IServerWorld, MxDispatcher>();
            _dispatcherChanged = new Dictionary<IWorld, bool>();
            _serverDispatcherChanged = new Dictionary<IServerWorld, bool>();
            _currentDispatchers = new MxDispatcher[1];
            _objectReferences = new Dictionary<int, WeakReference>();

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
            if (gameContext.World != _currentWorld || _shadowWorld == null)
            {
                if (_shadowWorld != null)
                {
                    _shadowWorld.Dispose();
                }

                _shadowWorld = _networkFactory.CreateShadowWorld();
                _currentWorld = gameContext.World;
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

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            if (serverContext.World != _currentServerWorld || _shadowWorld == null)
            {
                if (_shadowWorld != null)
                {
                    _shadowWorld.Dispose();
                }

                _shadowWorld = _networkFactory.CreateShadowWorld();
                _currentServerWorld = serverContext.World;
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

        public MxDispatcher[] CurrentDispatchers => _currentDispatchers;

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
    }
}
