namespace Protogame
{
    using Jitter;

    public interface IPhysicsEngine
    {
        void UpdateWorld(JitterWorld world, IGameContext gameContext, IUpdateContext updateContext);
    }
}