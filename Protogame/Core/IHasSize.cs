namespace Protogame
{
    /// <summary>
    /// An interface which indicates that an object has a bounding box size assocated with it.
    /// </summary>
    /// <remarks>
    /// This interface is currently only used by the platforming module, and may be deprecated in the future.
    /// </remarks>
    /// <module>Core API</module>
    public interface IHasSize
    {
        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        float Depth { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        float Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        float Width { get; set; }
    }
}