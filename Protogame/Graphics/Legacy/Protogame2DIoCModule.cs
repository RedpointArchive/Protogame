namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The Protoinject module to load when building a 2D game in Protogame.  This provides various
    /// bindings that are applicable to 2D games.
    /// </summary>
    [System.Obsolete("Use ProtogameCoreModule along with the new render pipeline")]
    public class Protogame2DIoCModule : IProtoinjectModule
    {
        /// <summary>
        /// An internal method called by the Protoinject module system.
        /// Use kernel.Load&lt;Protogame2DIoCModule&gt; to load this module.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<I2DRenderUtilities>().To<Default2DRenderUtilities>();
            kernel.Bind<IAudioUtilities>().To<DefaultAudioUtilities>();
            kernel.Bind<ITileUtilities>().To<DefaultTileUtilities>();
            kernel.Bind<IBoundingBoxUtilities>().To<DefaultBoundingBoxUtilities>();
            kernel.Bind<IGameContext>().To<DefaultGameContext>();
            kernel.Bind<IUpdateContext>().To<DefaultUpdateContext>();
            kernel.Bind<IRenderContext>().To<LegacyRenderContext>();
            kernel.Bind<IKeyboardStringReader>().To<DefaultKeyboardStringReader>();
            kernel.Bind<IConsole>().To<DefaultConsole>().InSingletonScope();
            kernel.Bind<ICommand>().To<ExitCommand>();
            kernel.Bind<ICommand>().To<HelpCommand>();
            kernel.Bind<ICommand>().To<GCCommand>();
            kernel.Bind<IStringSanitizer>().To<DefaultStringSanitizer>();
        }
    }
}