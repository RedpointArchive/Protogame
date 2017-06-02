using Microsoft.Xna.Framework.Input;
using System;

namespace Protogame
{
    /// <summary>
    /// Represents a keyboard key being pressed down for the first frame.
    /// </summary>
    /// <module>Events</module>
    [Serializable]
    public class KeyPressEvent : KeyboardEvent
    {
        /// <summary>
        /// Constructs a new default <see cref="KeyPressEvent"/>.
        /// </summary>
        public KeyPressEvent()
        {
        }

        /// <summary>
        /// Constructs a new <see cref="KeyPressEvent"/> from a <see cref="KeyboardState"/>.
        /// </summary>
        /// <param name="state">The MonoGame representation of the keyboard.</param>
        public KeyPressEvent(KeyboardState state) : base(state)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="KeyPressEvent"/> from an existing <see cref="KeyPressEvent"/>.
        /// </summary>
        /// <param name="keyboardEvent">The existing keyboard event.</param>
        public KeyPressEvent(KeyPressEvent keyboardEvent) : base(keyboardEvent)
        {
            Key = keyboardEvent.Key;
        }

        /// <summary>
        /// Clones the current keyboard event instance and returns a copy.
        /// </summary>
        /// <returns>A copy of the current keyboard event.</returns>
        public override KeyboardEvent Clone()
        {
            return new KeyPressEvent(this);
        }

        /// <summary>
        /// The key being pressed down for the first frame.
        /// </summary>
        public Keys Key { get; set; }
    }
}