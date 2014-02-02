namespace Protogame
{
    using Ninject.Modules;

    /// <summary>
    /// The protogame platforming io c module.
    /// </summary>
    public class ProtogamePlatformingIoCModule : NinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public override void Load()
        {
            this.Bind<IPlatforming>().To<DefaultPlatforming>();
        }
    }
}