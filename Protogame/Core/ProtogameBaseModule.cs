using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The base Protogame dependency injection module which is used by both
    /// <see cref="ProtogameCoreModule"/> and <see cref="ProtogameServerModule"/>
    /// to bind common services.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ProtogameCoreModule</interface_ref>
    public class ProtogameBaseModule : IProtoinjectModule
    {
        /// <summary>
        /// Do not directly call this method, use <see cref="ProtogameCoreModule"/> 
        /// or <see cref="ProtogameServerModule"/>.
        /// </summary>
        public virtual void Load(IKernel kernel)
        {
            kernel.Bind<ITileUtilities>().To<DefaultTileUtilities>();
            kernel.Bind<IBoundingBoxUtilities>().To<DefaultBoundingBoxUtilities>();
            kernel.Bind<IStringSanitizer>().To<DefaultStringSanitizer>();
            kernel.Bind<ITransformUtilities>().To<DefaultTransformUtilities>().InSingletonScope();
            kernel.Bind<IUniqueIdentifierAllocator>().To<DefaultUniqueIdentifierAllocator>().InSingletonScope();
            kernel.Bind<IRenderCache>().To<DefaultRenderCache>().InSingletonScope();
            kernel.Bind<IRenderAutoCache>().To<DefaultRenderAutoCache>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<RenderAutoCacheEngineHook>();
        }
    }
}

