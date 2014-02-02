namespace Protogame
{
    /// <summary>
    /// The mouse move event.
    /// </summary>
    public class MouseMoveEvent : MouseEvent
    {
        /// <summary>
        /// Gets or sets the last x.
        /// </summary>
        /// <value>
        /// The last x.
        /// </value>
        public int LastX { get; set; }

        /// <summary>
        /// Gets or sets the last y.
        /// </summary>
        /// <value>
        /// The last y.
        /// </value>
        public int LastY { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public int Y { get; set; }
    }
}