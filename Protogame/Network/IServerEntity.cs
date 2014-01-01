namespace Protogame
{
    using System.IO;

    /// <summary>
    /// The server entity interface, which defines an interface for server-based entities.
    /// </summary>
    public interface IServerEntity : IHasPosition
    {
        /// <summary>
        /// Applies a delta from a binary stream onto the current entity.
        /// </summary>
        /// <param name="reader">
        /// The binary reader.
        /// </param>
        void ApplyDelta(BinaryReader reader);

        /// <summary>
        /// Calculates a delta from the specified entity to this entity, and writes it into the binary writer.
        /// </summary>
        /// <param name="writer">
        /// The binary writer.
        /// </param>
        /// <param name="fromOther">
        /// The other version to calculate from.
        /// </param>
        void CalculateDelta(BinaryWriter writer, IServerEntity fromOther);

        /// <summary>
        /// Takes a snapshot of this entity and returns it.  The resulting object should be a copy
        /// of this class, not a reference to it.
        /// </summary>
        /// <returns>
        /// A copy of the current <see cref="IServerEntity"/>.
        /// </returns>
        IServerEntity Snapshot();

        /// <summary>
        /// Updates this server entity.
        /// </summary>
        void Update();
    }
}