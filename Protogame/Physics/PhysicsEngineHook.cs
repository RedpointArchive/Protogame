namespace Protogame
{
    public class PhysicsEngineHook : IEngineHook
    {
        private readonly IPhysicsEngine _physicsEngine;

        public PhysicsEngineHook(IPhysicsEngine physicsEngine)
        {
            _physicsEngine = physicsEngine;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            _physicsEngine.DebugRender(gameContext, renderContext);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _physicsEngine.Update(gameContext, updateContext);
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            _physicsEngine.Update(serverContext, updateContext);
        }
    }
}
