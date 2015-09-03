namespace Protogame
{
    /// <summary>
    /// The LevelManager interface.
    /// </summary>
    /// <module>Level</module>
    public interface ILevelManager
    {
        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="world">
        /// The world.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        void Load(IWorld world, string name);
    }
}