namespace Protogame
{
    using Ninject.Modules;

    /// <summary>
    /// The Ninject module to load when using scripting assets.
    /// </summary>
    public class ProtogameScriptIoCModule : NinjectModule
    {
        /// <summary>
        /// An internal method called by the Ninject module system.
        /// Use kernel.Load&lt;ProtogameScriptIoCModule&gt; to load this module.
        /// </summary>
        public override void Load()
        {
            this.Bind<IAssetLoader>().To<LogicControlScriptAssetLoader>();
            this.Bind<IAssetSaver>().To<LogicControlScriptAssetSaver>();

            this.Bind<ILoadStrategy>().To<RawLogicControlScriptLoadStrategy>();
        }
    }
}