namespace Protogame
{
    /// <summary>
    /// The touch release event.
    /// </summary>
    public class TouchReleaseEvent : TouchEvent
    {
        /// <summary>
        /// Gets or sets the X position of the touch release.
        /// </summary>
        /// <value>The X position of the touch release.</value>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the release release.
        /// </summary>
        /// <value>The Y position of the touch release.</value>
        public float Y { get; set; }
    }
}