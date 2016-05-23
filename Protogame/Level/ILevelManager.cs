using System;

namespace Protogame
{
    /// <summary>
    /// The LevelManager interface.
    /// </summary>
    /// <module>Level</module>
    public interface ILevelManager
    {
        /// <summary>
        /// Loads a level entity into the game hierarchy, with the specified
        /// context as the place to load entities.  Normally you'll pass in the
        /// game world here, but you don't have to.  For example, if you wanted to
        /// load the level into an entity group, you would pass the entity group
        /// as the context instead.
        /// </summary>
        /// <param name="context">Usually the current game world, but can be any object in the hierarchy.</param>
        /// <param name="levelAsset">The level to load.</param>
        void Load(object context, LevelAsset levelAsset);

        [Obsolete("Use one of the other Load methods instead.")]
        void Load(IWorld world, string name);
    }
}