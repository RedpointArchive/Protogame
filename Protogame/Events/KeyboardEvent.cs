namespace Protogame
{
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The keyboard event.
    /// </summary>
    public class KeyboardEvent : Event
    {
        /// <summary>
        /// Gets or sets the keyboard state.
        /// </summary>
        /// <value>
        /// The keyboard state.
        /// </value>
        public KeyboardState KeyboardState { get; set; }
    }
}