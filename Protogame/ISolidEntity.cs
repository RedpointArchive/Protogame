namespace Protogame
{
    /// <summary>
    /// The SolidEntity interface.
    /// </summary>
    public interface ISolidEntity : IEntity
    {
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