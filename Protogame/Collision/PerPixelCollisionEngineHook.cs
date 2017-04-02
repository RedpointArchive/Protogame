namespace Protogame
{
    public class PerPixelCollisionEngineHook : IEngineHook
    {
        private readonly IPerPixelCollisionEngine _collisionEngine;

        public PerPixelCollisionEngineHook(IPerPixelCollisionEngine collisionEngine)
        {
            _collisionEngine = collisionEngine;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            _collisionEngine.DebugRender(gameContext, renderContext);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _collisionEngine.Update(gameContext, updateContext);
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            _collisionEngine.Update(serverContext, updateContext);
        }
    }
}
