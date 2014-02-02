namespace Protogame
{
    /// <summary>
    /// The HasVelocity interface.
    /// </summary>
    public interface IHasVelocity
    {
        /// <summary>
        /// Gets or sets the x speed.
        /// </summary>
        /// <value>
        /// The x speed.
        /// </value>
        float XSpeed { get; set; }

        /// <summary>
        /// Gets or sets the y speed.
        /// </summary>
        /// <value>
        /// The y speed.
        /// </value>
        float YSpeed { get; set; }

        /// <summary>
        /// Gets or sets the z speed.
        /// </summary>
        /// <value>
        /// The z speed.
        /// </value>
        float ZSpeed { get; set; }
    }
}