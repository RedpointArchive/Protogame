namespace Protogame
{
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The key press event.
    /// </summary>
    public class KeyReleaseEvent : KeyboardEvent
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public Keys Key { get; set; }
    }
}