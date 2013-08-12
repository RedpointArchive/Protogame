using Ninject.Modules;

namespace Protogame
{
    public class Protogame2DIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<I2DRenderUtilities>().To<Default2DRenderUtilities>();
            this.Bind<IAudioUtilities>().To<DefaultAudioUtilities>();
            this.Bind<ITileUtilities>().To<DefaultTileUtilities>();
            this.Bind<IBoundingBoxUtilities>().To<DefaultBoundingBoxUtilities>();
            this.Bind<IGameContext>().To<DefaultGameContext>();
            this.Bind<IUpdateContext>().To<DefaultUpdateContext>();
            this.Bind<IRenderContext>().To<DefaultRenderContext>();
            this.Bind<IKeyboardStringReader>().To<DefaultKeyboardStringReader>();
            this.Bind<IConsole>().To<DefaultConsole>().InSingletonScope();
            this.Bind<ICommand>().To<ExitCommand>();
            this.Bind<ICommand>().To<HelpCommand>();
        }
    }
}

