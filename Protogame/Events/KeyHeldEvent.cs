namespace Protogame
{
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The key held event.
    /// </summary>
    public class KeyHeldEvent : KeyboardEvent
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