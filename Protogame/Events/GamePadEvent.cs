namespace Protogame
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Represents an event that occurred on a game pad input device.
    /// </summary>
    public abstract class GamePadEvent : Event
    {
        /// <summary>
        /// Gets or sets the game pad state.
        /// </summary>
        /// <value>
        /// The game pad state.
        /// </value>
        public GamePadState GamePadState { get; set; }

        /// <summary>
        /// Gets or sets the game pad index.
        /// </summary>
        /// <value>
        /// The game pad index.
        /// </value>
        public int GamePadIndex { get; set; }
    }
}