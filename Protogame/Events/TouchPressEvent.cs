namespace Protogame
{
    /// <summary>
    /// The touch press event.
    /// </summary>
    public class TouchPressEvent : TouchEvent
    {
        /// <summary>
        /// Gets or sets the X position of the touch press.
        /// </summary>
        /// <value>The X position of the touch press.</value>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the touch press.
        /// </summary>
        /// <value>The Y position of the touch press.</value>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the pressure of the touch event.
        /// </summary>
        /// <value>The touch pressure.</value>
        public float Pressure { get; set; }
    }
}