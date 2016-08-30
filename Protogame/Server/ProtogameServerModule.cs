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

            kernel.Bind<IServerContext>().To<DefaultServerContext>();
            kernel.Bind<IUpdateContext>().To<DefaultUpdateContext>();

            // Binds all of the services normally available under ProtogameCoreModule to
            // dummy implementations, because the GPU and various other API calls that
            // would normally be available with the MonoGame game framework running aren't
            // available in a server.
            LoadNullServices(kernel);
        }

        private void LoadNullServices(IKernel kernel)
        {
            kernel.Bind<IAssetContentManager>().To<NullAssetContentManager>().InSingletonScope();

            kernel.Bind<I2DRenderUtilities>().To<Null2DRenderUtilities>().InSingletonScope();
            kernel.Bind<I3DRenderUtilities>().To<Null3DRenderUtilities>().InSingletonScope();
            kernel.Bind<IAudioUtilities>().To<NullAudioUtilities>().InSingletonScope();
            kernel.Bind<IKeyboardStringReader>().To<NullKeyboardStringReader>().InSingletonScope();
            kernel.Bind<IConsole>().To<ServerConsole>().InSingletonScope();
            kernel.Bind<IConsoleHandle>().To<DefaultConsoleHandle>().InParentScope();

            kernel.Bind<IRenderContext>().To<NullRenderContext>();
            kernel.Bind<IRenderBatcher>().To<NullRenderBatcher>();

            kernel.Bind<IGraphicsFactory>().ToFactoryNotSupported();
            kernel.Bind<ILightFactory>().ToFactoryNotSupported();
        }
    }
}

