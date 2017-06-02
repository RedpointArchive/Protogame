using Microsoft.Xna.Framework.Input;
using System;

namespace Protogame
{
    /// <summary>
    /// Represents a keyboard key being released.
    /// </summary>
    /// <module>Events</module>
    [Serializable]
    public class KeyReleaseEvent : KeyboardEvent
    {
        /// <summary>
        /// Constructs a new default <see cref="KeyReleaseEvent"/>.
        /// </summary>
        public KeyReleaseEvent()
        {
        }

        /// <summary>
        /// Constructs a new <see cref="KeyReleaseEvent"/> from a <see cref="KeyboardState"/>.
        /// </summary>
        /// <param name="state">The MonoGame representation of the keyboard.</param>
        public KeyReleaseEvent(KeyboardState state) : base(state)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="KeyReleaseEvent"/> from an existing <see cref="KeyReleaseEvent"/>.
        /// </summary>
        /// <param name="keyboardEvent">The existing keyboard event.</param>
        public KeyReleaseEvent(KeyReleaseEvent keyboardEvent) : base(keyboardEvent)
        {
            Key = keyboardEvent.Key;
        }

        /// <summary>
        /// Clones the current keyboard event instance and returns a copy.
        /// </summary>
        /// <returns>A copy of the current keyboard event.</returns>
        public override KeyboardEvent Clone()
        {
            return new KeyReleaseEvent(this);
        }

        /// <summary>
        /// The key that was released.
        /// </summary>
        public Keys Key { get; set; }
    }
}