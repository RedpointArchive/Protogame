using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The Protoinject module to load when using the server services in Protogame.
    /// </summary>
    public class ProtogameServerModule : ProtogameBaseModule
    {
        /// <summary>
        /// You should call <see cref="Protoinject.StandardKernel.Load{ProtogameCoreModule}"/> 
        /// instead of calling this method directly.
        /// </summary>
        public override void Load(IKernel kernel)
        {
            base.Load(kernel);

            kernel.Bind<ITickRegulator>().To<DefaultTickRegulator>().InSingletonScope();
            kernel.Bind<IUniqueIdentifierAllocator>().To<DefaultUniqueIdentifierAllocator>().InSingletonScope();

            kernel.Bind<IServerContext>().To<DefaultServerContext>();
            kernel.Bind<IUpdateContext>().To<DefaultUpdateContext>();
        }
    }
}

