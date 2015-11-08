namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The Protoinject module to load when using scripting assets.
    /// </summary>
    public class ProtogameScriptIoCModule : IProtoinjectModule
    {
        /// <summary>
        /// An internal method called by the Protoinject module system.
        /// Use kernel.Load&lt;ProtogameScriptIoCModule&gt; to load this module.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<IAssetLoader>().To<LogicControlScriptAssetLoader>();
            kernel.Bind<IAssetSaver>().To<LogicControlScriptAssetSaver>();

            kernel.Bind<ILoadStrategy>().To<RawLogicControlScriptLoadStrategy>();
        }
    }
}