namespace Protogame
{
    public interface IPerPixelCollisionEngine
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext);

        void Update(IServerContext serverContext, IUpdateContext updateContext);

        void UnregisterComponentInCurrentWorld(IPerPixelCollisionComponent perPixelCollisionComponent);

        void RegisterComponentInCurrentWorld(IPerPixelCollisionComponent perPixelCollisionComponent);
        void DebugRender(IGameContext gameContext, IRenderContext renderContext);
    }
}
