namespace Protogame
{
    /// <summary>
    /// The HasPosition interface.
    /// </summary>
    public interface IHasPosition
    {
        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        float X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        float Y { get; set; }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        /// <value>
        /// The z.
        /// </value>
        float Z { get; set; }
    }
}