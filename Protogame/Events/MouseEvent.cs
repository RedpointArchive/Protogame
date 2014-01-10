namespace Protogame
{
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The mouse event.
    /// </summary>
    public class MouseEvent : Event
    {
        /// <summary>
        /// Gets or sets the mouse state.
        /// </summary>
        /// <value>
        /// The mouse state.
        /// </value>
        public MouseState MouseState { get; set; }
    }
}