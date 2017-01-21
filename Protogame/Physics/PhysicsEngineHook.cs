namespace Protogame
{
    public class PhysicsEngineHook : IEngineHook
    {
        private readonly IPhysicsEngine _physicsEngine;
        private readonly IPhysicsWorldControl _physicsWorldControl;

        public PhysicsEngineHook(IPhysicsEngine physicsEngine, IPhysicsWorldControl physicsWorldControl)
        {
            _physicsEngine = physicsEngine;
            _physicsWorldControl = physicsWorldControl;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            _physicsEngine.DebugRender(gameContext, renderContext);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _physicsEngine.Update(gameContext, updateContext);
            _physicsWorldControl.SyncPendingChanges();
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            _physicsEngine.Update(serverContext, updateContext);
            _physicsWorldControl.SyncPendingChanges();
        }
    }
}
