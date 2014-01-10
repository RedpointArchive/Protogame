namespace Protogame
{
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Represents an event relating to a button on a game pad input device.
    /// </summary>
    public abstract class GamePadButtonEvent : GamePadEvent
    {
        /// <summary>
        /// Gets or sets the button related to the event.
        /// </summary>
        /// <value>
        /// The button related to the event.
        /// </value>
        public Buttons Button { get; set; }
    }
}