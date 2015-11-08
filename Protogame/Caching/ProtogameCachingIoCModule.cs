namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The protogame caching io c module.
    /// </summary>
    public class ProtogameCachingIoCModule : IProtoinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<IRenderCache>().To<DefaultRenderCache>().InSingletonScope();
        }
    }
}