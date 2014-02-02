namespace Protogame
{
    using Ninject.Modules;

    /// <summary>
    /// The protogame caching io c module.
    /// </summary>
    public class ProtogameCachingIoCModule : NinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public override void Load()
        {
            this.Bind<IRenderCache>().To<DefaultRenderCache>().InSingletonScope();
        }
    }
}