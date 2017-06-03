using Microsoft.Xna.Framework.Input;
using System;

namespace Protogame
{
    /// <summary>
    /// Represents a keyboard key being held down for more than one frame.
    /// </summary>
    /// <module>Events</module>
    [Serializable]
    public class KeyHeldEvent : KeyboardEvent
    {
        /// <summary>
        /// Constructs a new default <see cref="KeyHeldEvent"/>.
        /// </summary>
        public KeyHeldEvent()
        {
        }

        /// <summary>
        /// Constructs a new <see cref="KeyHeldEvent"/> from a <see cref="KeyboardState"/>.
        /// </summary>
        /// <param name="state">The MonoGame representation of the keyboard.</param>
        public KeyHeldEvent(KeyboardState state) : base(state)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="KeyHeldEvent"/> from an existing <see cref="KeyHeldEvent"/>.
        /// </summary>
        /// <param name="keyboardEvent">The existing keyboard event.</param>
        public KeyHeldEvent(KeyHeldEvent keyboardEvent) : base(keyboardEvent)
        {
            Key = keyboardEvent.Key;
        }

        /// <summary>
        /// Clones the current keyboard event instance and returns a copy.
        /// </summary>
        /// <returns>A copy of the current keyboard event.</returns>
        public override KeyboardEvent Clone()
        {
            return new KeyHeldEvent(this);
        }

        /// <summary>
        /// The key that is being held down for more than one frame.
        /// </summary>
        public Keys Key { get; set; }
    }
}