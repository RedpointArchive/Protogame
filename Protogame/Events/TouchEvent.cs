namespace Protogame
{
    using Microsoft.Xna.Framework.Input.Touch;

    /// <summary>
    /// The touch event.
    /// </summary>
    public class TouchEvent : Event
    {
        /// <summary>
        /// Gets or sets the touch location state.
        /// </summary>
        /// <value>
        /// The touch location state.
        /// </value>
        public TouchLocationState TouchLocationState { get; set; }
    }
}