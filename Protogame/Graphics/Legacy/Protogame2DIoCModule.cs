namespace Protogame
{
    using Ninject.Modules;

    /// <summary>
    /// The Ninject module to load when building a 2D game in Protogame.  This provides various
    /// bindings that are applicable to 2D games.
    /// </summary>
    [System.Obsolete("Use ProtogameCoreModule along with the new render pipeline")]
    public class Protogame2DIoCModule : NinjectModule
    {
        /// <summary>
        /// An internal method called by the Ninject module system.
        /// Use kernel.Load&lt;Protogame2DIoCModule&gt; to load this module.
        /// </summary>
        public override void Load()
        {
            this.Bind<I2DRenderUtilities>().To<Default2DRenderUtilities>();
            this.Bind<IAudioUtilities>().To<DefaultAudioUtilities>();
            this.Bind<ITileUtilities>().To<DefaultTileUtilities>();
            this.Bind<IBoundingBoxUtilities>().To<DefaultBoundingBoxUtilities>();
            this.Bind<IGameContext>().To<DefaultGameContext>();
            this.Bind<IUpdateContext>().To<DefaultUpdateContext>();
            this.Bind<IRenderContext>().To<LegacyRenderContext>();
            this.Bind<IKeyboardStringReader>().To<DefaultKeyboardStringReader>();
            this.Bind<IConsole>().To<DefaultConsole>().InSingletonScope();
            this.Bind<ICommand>().To<ExitCommand>();
            this.Bind<ICommand>().To<HelpCommand>();
            this.Bind<ICommand>().To<GCCommand>();
            this.Bind<IStringSanitizer>().To<DefaultStringSanitizer>();
        }
    }
}