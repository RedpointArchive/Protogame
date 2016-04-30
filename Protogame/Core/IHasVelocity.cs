namespace Protogame
{
    /// <summary>
    /// An interface which indicates that an object has a velocity assocated with it.
    /// </summary>
    /// <remarks>
    /// This interface is currently only used by the platforming module, and may be deprecated in the future.
    /// </remarks>
    /// <module>Core API</module>
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