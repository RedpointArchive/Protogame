namespace Protogame
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The level reader interface that supplies services for
    /// reading level formats and returning a list of entities.
    /// </summary>
    /// <module>Level</module>
    public interface ILevelReader
    {
        /// <summary>
        /// Read the specified stream and return a list of constructed entities.
        /// </summary>
        /// <param name="stream">The stream which contains level data.</param>
        /// <param name="context">
        /// The context in which entities are being spawned in the hierarchy.  This is
        /// usually the current world, but it doesn't have to be (e.g. if you wanted to
        /// load a level under an entity group, you would pass the entity group here).
        /// </param>
        /// <returns>A list of entities to spawn within the world.</returns>
        IEnumerable<IEntity> Read(Stream stream, object context);
    }
}