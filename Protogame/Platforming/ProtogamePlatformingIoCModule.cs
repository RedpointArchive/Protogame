namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The protogame platforming io c module.
    /// </summary>
    public class ProtogamePlatformingIoCModule : IProtoinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<IPlatforming>().To<DefaultPlatforming>();
        }
    }
}