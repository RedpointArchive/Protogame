namespace Protogame
{
    using Jitter;

    public class DefaultPhysicsEngine : IPhysicsEngine
    {
        public void UpdateWorld(JitterWorld world, IGameContext gameContext, IUpdateContext updateContext)
        {
            world.Step((float)gameContext.GameTime.ElapsedGameTime.TotalSeconds, true);
        }
    }
}