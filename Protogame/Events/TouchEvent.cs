namespace Protogame
{
    using Microsoft.Xna.Framework.Input.Touch;

    /// <summary>
    /// The touch event.
    /// </summary>
    public class TouchEvent : Event
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
        /// Gets or sets the pressure of the touch.
        /// </summary>
        public float Pressure { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the touch.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the touch location state.
        /// </summary>
        /// <value>
        /// The touch location state.
        /// </value>
        public TouchLocationState TouchLocationState { get; set; }
    }
}