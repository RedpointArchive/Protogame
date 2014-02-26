namespace Protogame
{
    using Ninject.Modules;

    /// <summary>
    /// The Ninject module to load when using the physics services in Protogame.
    /// </summary>
    public class ProtogamePhysicsIoCModule : NinjectModule
    {
        /// <summary>
        /// An internal method called by the Ninject module system.
        /// Use kernel.Load&lt;ProtogamePhysicsIoCModule&gt; to load this module.
        /// </summary>
        public override void Load()
        {
            this.Bind<IPhysicsEngine>().To<DefaultPhysicsEngine>();
        }
    }
}
