namespace Protogame
{
    using Jitter;

    /// <summary>
    /// The physics engine service.
    /// </summary>
    /// <module>Physics</module>
    public interface IPhysicsEngine
    {
        void UpdateWorld(JitterWorld world, IGameContext gameContext, IUpdateContext updateContext);
    }
}