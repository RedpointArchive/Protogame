namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The level loading module, which provides functionality for loading game levels
    /// into the current world.
    /// </summary>
    /// <module>Level</module>
    public class ProtogameLevelModule : IProtoinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<ITileset>().To<DefaultTileset>();
            kernel.Bind<ILevelManager>().To<DefaultLevelManager>();
            kernel.Bind<ILevelReader>().To<OgmoLevelReader>().Named(LevelDataFormat.OgmoEditor.ToString()).EnforceOnePerScope();
            kernel.Bind<ILevelReader>().To<ATFLevelReader>().Named(LevelDataFormat.ATF.ToString()).EnforceOnePerScope();
        }
    }
}