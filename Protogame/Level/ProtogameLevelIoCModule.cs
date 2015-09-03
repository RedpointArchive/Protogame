namespace Protogame
{
    using Ninject.Modules;

    /// <summary>
    /// The protogame level io c module.
    /// </summary>
    /// <module>Level</module>
    public class ProtogameLevelIoCModule : NinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public override void Load()
        {
            this.Bind<ITileset>().To<DefaultTileset>();
            this.Bind<ILevelManager>().To<DefaultLevelManager>();
            this.Bind<ILevelReader>().To<OgmoLevelReader>();
        }
    }
}