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
        /// <param name="stream">
        /// The stream to read from.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable&lt;IEntity&gt;"/> read from the stream.
        /// </returns>
        IEnumerable<IEntity> Read(Stream stream);
    }
}