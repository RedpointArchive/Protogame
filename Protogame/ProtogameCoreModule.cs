using Ninject.Modules;

namespace Protogame
{
    /// <summary>
    /// The core Protogame dependency injection module, which loads all of the core
    /// classes required for basic game functionality.  You must load this module
    /// for Protogame to work.
    /// </summary>
    /// <module>Core API</module>
    public class ProtogameCoreModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<I2DRenderUtilities>().To<Default2DRenderUtilities>();
            this.Bind<I3DRenderUtilities>().To<Default3DRenderUtilities>();
            this.Bind<IAudioUtilities>().To<DefaultAudioUtilities>();
            this.Bind<ITileUtilities>().To<DefaultTileUtilities>();
            this.Bind<IBoundingBoxUtilities>().To<DefaultBoundingBoxUtilities>();
            this.Bind<IGameContext>().To<DefaultGameContext>();
            this.Bind<IUpdateContext>().To<DefaultUpdateContext>();
            this.Bind<IKeyboardStringReader>().To<DefaultKeyboardStringReader>();
            this.Bind<IConsole>().To<DefaultConsole>().InSingletonScope();
            this.Bind<ICommand>().To<ExitCommand>();
            this.Bind<ICommand>().To<HelpCommand>();
            this.Bind<ICommand>().To<GCCommand>();
            this.Bind<IStringSanitizer>().To<DefaultStringSanitizer>();

            this.Bind<IRenderContext>().To<RenderPipelineRenderContext>();
            this.Bind<IRenderPipeline>().To<DefaultRenderPipeline>();
            this.Bind<IGraphicsBlit>().To<DefaultGraphicsBlit>();
        }
    }
}

