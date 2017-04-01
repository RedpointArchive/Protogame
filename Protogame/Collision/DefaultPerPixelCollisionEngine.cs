namespace Protogame
{
    public class DefaultPerPixelCollisionEngine : IPerPixelCollisionEngine
    {
        private readonly IPerPixelCollisionFactory _collisionFactory;
        private readonly IProfiler _profiler;
        private IWorld _currentWorld;
        private IServerWorld _currentServerWorld;
        private PerPixelCollisionShadowWorld _shadowWorld;

        public DefaultPerPixelCollisionEngine(
            IPerPixelCollisionFactory collisionFactory,
            IProfiler profiler)
        {
            _collisionFactory = collisionFactory;
            _profiler = profiler;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (gameContext.World == null)
            {
                return;
            }

            if (gameContext.World != _currentWorld || _shadowWorld == null)
            {
                if (_shadowWorld != null)
                {
                    _shadowWorld.Dispose();
                }

                _shadowWorld = _collisionFactory.CreateShadowWorld();
                _currentWorld = gameContext.World;
            }

            using (_profiler.Measure("collision-step"))
            {
                _shadowWorld.Update(gameContext, updateContext);
            }
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            if (serverContext.World == null)
            {
                return;
            }

            if (serverContext.World != _currentServerWorld || _shadowWorld == null)
            {
                if (_shadowWorld != null)
                {
                    _shadowWorld.Dispose();
                }

                _shadowWorld = _collisionFactory.CreateShadowWorld();
                _currentServerWorld = serverContext.World;
            }

            using (_profiler.Measure("collision-step"))
            {
                _shadowWorld.Update(serverContext, updateContext);
            }
        }

        public void DebugRender(IGameContext gameContext, IRenderContext renderContext)
        {
            if (gameContext.World == null)
            {
                return;
            }

            if (gameContext.World != _currentWorld || _shadowWorld == null)
            {
                if (_shadowWorld != null)
                {
                    _shadowWorld.Dispose();
                }

                _shadowWorld = _collisionFactory.CreateShadowWorld();
                _currentWorld = gameContext.World;
            }

            using (_profiler.Measure("collision-debug-render"))
            {
                _shadowWorld.DebugRender(gameContext, renderContext);
            }
        }

        public void UnregisterComponentInCurrentWorld(IPerPixelCollisionComponent perPixelCollidableComponent)
        {
            if (_shadowWorld != null)
            {
                _shadowWorld.UnregisterComponentInCurrentWorld(perPixelCollidableComponent);
            }
        }

        public void RegisterComponentInCurrentWorld(IPerPixelCollisionComponent perPixelCollidableComponent)
        {
            if (_shadowWorld != null)
            {
                _shadowWorld.RegisterComponentInCurrentWorld(perPixelCollidableComponent);
            }
        }
    }
}
