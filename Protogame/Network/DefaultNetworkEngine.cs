using System.Collections.Generic;

namespace Protogame
{
    public class DefaultNetworkEngine : INetworkEngine
    {
        private readonly INetworkFactory _networkFactory;
        private IWorld _currentWorld;
        private IServerWorld _currentServerWorld;
        private NetworkShadowWorld _shadowWorld;

        private readonly Dictionary<IWorld, MxDispatcher> _dispatchers;
        private readonly Dictionary<IServerWorld, MxDispatcher> _serverDispatchers;
        private readonly Dictionary<IWorld, bool> _dispatcherChanged;
        private readonly Dictionary<IServerWorld, bool> _serverDispatcherChanged;

        private readonly MxDispatcher[] _currentDispatchers;

        public DefaultNetworkEngine(INetworkFactory networkFactory)
        {
            _networkFactory = networkFactory;
            _dispatchers = new Dictionary<IWorld, MxDispatcher>();
            _serverDispatchers = new Dictionary<IServerWorld, MxDispatcher>();
            _dispatcherChanged = new Dictionary<IWorld, bool>();
            _serverDispatcherChanged = new Dictionary<IServerWorld, bool>();
            _currentDispatchers = new MxDispatcher[1];
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

            _shadowWorld.Update(gameContext, updateContext);
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

            _shadowWorld.Update(serverContext, updateContext);
        }

        public MxDispatcher[] CurrentDispatchers => _currentDispatchers;
    }
}
