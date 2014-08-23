namespace Protogame
{
    /// <summary>
    /// The touch held event.
    /// </summary>
    public class TouchHeldEvent : TouchEvent
    {
        /// <summary>
        /// Gets or sets the X position of the touch.
        /// </summary>
        /// <value>The X position of the touch.</value>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the touch.
        /// </summary>
        /// <value>The Y position of the touch.</value>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the pressure of the touch event.
        /// </summary>
        /// <value>The touch pressure.</value>
        public float Pressure { get; set; }
    }
}