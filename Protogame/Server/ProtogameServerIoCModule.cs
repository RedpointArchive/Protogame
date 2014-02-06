namespace Protogame
{
    using Ninject.Modules;

    /// <summary>
    /// The Ninject module to load when using the server services in Protogame.
    /// </summary>
    public class ProtogameServerIoCModule : NinjectModule
    {
        /// <summary>
        /// An internal method called by the Ninject module system.
        /// Use kernel.Load&lt;ProtogameServerIoCModule&gt; to load this module.
        /// </summary>
        public override void Load()
        {
            this.Bind<ITickRegulator>().To<DefaultTickRegulator>().InSingletonScope();
            this.Bind<IUniqueIdentifierAllocator>().To<DefaultUniqueIdentifierAllocator>().InSingletonScope();

            this.Bind<IServerContext>().To<DefaultServerContext>();
            this.Bind<IUpdateContext>().To<DefaultUpdateContext>();
        }
    }
}

