namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The protogame level io c module.
    /// </summary>
    /// <module>Level</module>
    public class ProtogameLevelIoCModule : IProtoinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<ITileset>().To<DefaultTileset>();
            kernel.Bind<ILevelManager>().To<DefaultLevelManager>();
            kernel.Bind<ILevelReader>().To<OgmoLevelReader>();
        }
    }
}