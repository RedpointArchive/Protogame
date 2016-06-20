namespace Protogame
{
    /// <summary>
    /// An interface which indicates that an object has a velocity associated with it.
    /// </summary>
    /// <remarks>
    /// This interface is currently only used by the platforming module, and may be deprecated in the future.
    /// </remarks>
    /// <module>Core API</module>
    public interface IHasVelocity
    {
        /// <summary>
        /// Gets or sets the X speed.
        /// </summary>
        /// <value>
        /// The X speed.
        /// </value>
        float XSpeed { get; set; }

        /// <summary>
        /// Gets or sets the Y speed.
        /// </summary>
        /// <value>
        /// The Y speed.
        /// </value>
        float YSpeed { get; set; }

        /// <summary>
        /// Gets or sets the Z speed.
        /// </summary>
        /// <value>
        /// The Z speed.
        /// </value>
        float ZSpeed { get; set; }
    }
}