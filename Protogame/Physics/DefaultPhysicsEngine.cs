namespace Protogame
{
    using Jitter;

    /// <summary>
    /// The default implementation of a physics engine.
    /// </summary>
    /// <module>Physics</module>
    /// <internal>True</internal>
    /// <interface_ref>IPhysicsEngine</interface_ref>
    public class DefaultPhysicsEngine : IPhysicsEngine
    {
        public void UpdateWorld(JitterWorld world, IGameContext gameContext, IUpdateContext updateContext)
        {
            world.Step((float)gameContext.GameTime.ElapsedGameTime.TotalSeconds, true);
        }
    }
}