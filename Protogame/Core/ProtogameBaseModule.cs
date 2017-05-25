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
            kernel.Bind<ITileUtilities>().To<DefaultTileUtilities>().DiscardNodeOnResolve();
            kernel.Bind<IBoundingBoxUtilities>().To<DefaultBoundingBoxUtilities>().InSingletonScope();
            kernel.Bind<IStringSanitizer>().To<DefaultStringSanitizer>().InSingletonScope();
            kernel.Bind<ITransformUtilities>().To<DefaultTransformUtilities>().InSingletonScope();
            kernel.Bind<IUniqueIdentifierAllocator>().To<DefaultUniqueIdentifierAllocator>().InSingletonScope();
            kernel.Bind<IRenderCache>().To<DefaultRenderCache>().InSingletonScope();
            kernel.Bind<IRenderAutoCache>().To<DefaultRenderAutoCache>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<RenderAutoCacheEngineHook>().InParentScope();
#if DEBUG
            kernel.Bind<IRemoteClient>().To<DefaultRemoteClient>().InSingletonScope();
            kernel.Bind<ILogShipping>().To<DefaultLogShipping>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<ConsoleLogShippingEngineHook>();
#else
            kernel.Bind<IRemoteClient>().To<NullRemoteClient>().InSingletonScope();
            kernel.Bind<ILogShipping>().To<NullLogShipping>().InSingletonScope();
#endif
            kernel.Bind<ICoroutineScheduler>().To<DefaultCoroutineScheduler>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<CoroutineEngineHook>();
            kernel.Bind<ICoroutine>().To<DefaultCoroutine>();
        }
    }
}

