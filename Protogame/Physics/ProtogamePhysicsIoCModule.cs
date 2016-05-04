namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The Protoinject module to load when using the physics services in Protogame.
    /// </summary>
    public class ProtogamePhysicsIoCModule : IProtoinjectModule
    {
        /// <summary>
        /// An internal method called by the Protoinject module system.
        /// Use kernel.Load&lt;ProtogamePhysicsIoCModule&gt; to load this module.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<IPhysicsEngine>().To<DefaultPhysicsEngine>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<PhysicsEngineHook>();
            kernel.Bind<IPhysicsFactory>().ToFactory();
        }
    }
}
