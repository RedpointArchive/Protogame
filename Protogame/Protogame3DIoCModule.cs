namespace Protogame
{
    using Ninject.Modules;

    /// <summary>
    /// The Ninject module to load when building a 3D game in Protogame.  This provides various
    /// bindings that are applicable to 3D games.
    /// </summary>
    public class Protogame3DIoCModule : NinjectModule
    {
        /// <summary>
        /// An internal method called by the Ninject module system.
        /// Use kernel.Load&lt;Protogame3DIoCModule&gt; to load this module.
        /// </summary>
        public override void Load()
        {
            this.Bind<I2DRenderUtilities>().To<Default2DRenderUtilities>();
            this.Bind<I3DRenderUtilities>().To<Default3DRenderUtilities>();
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
            this.Bind<ICommand>().To<GCCommand>();
            this.Bind<IStringSanitizer>().To<DefaultStringSanitizer>();
        }
    }
}