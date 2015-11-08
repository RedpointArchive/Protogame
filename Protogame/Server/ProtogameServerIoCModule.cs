namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The Protoinject module to load when using the server services in Protogame.
    /// </summary>
    public class ProtogameServerIoCModule : IProtoinjectModule
    {
        /// <summary>
        /// An internal method called by the Protoinject module system.
        /// Use kernel.Load&lt;ProtogameServerIoCModule&gt; to load this module.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<ITickRegulator>().To<DefaultTickRegulator>().InSingletonScope();
            kernel.Bind<IUniqueIdentifierAllocator>().To<DefaultUniqueIdentifierAllocator>().InSingletonScope();

            kernel.Bind<IServerContext>().To<DefaultServerContext>();
            kernel.Bind<IUpdateContext>().To<DefaultUpdateContext>();
        }
    }
}

