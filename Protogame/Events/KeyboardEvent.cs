using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Protogame
{
    /// <summary>
    /// Represents a keyboard event.
    /// </summary>
    /// <module>Events</module>
    [Serializable]
    public class KeyboardEvent : Event
    {
        [NonSerialized]
        private HashSet<Keys> _pressedKeysHashSet;
        private Keys[] _pressedKeysArray;

        /// <summary>
        /// Constructs a new default <see cref="KeyboardEvent"/>.
        /// </summary>
        public KeyboardEvent()
        {
            _pressedKeysArray = new Keys[0];
            _pressedKeysHashSet = new HashSet<Keys>();
        }
        
        /// <summary>
        /// Constructs a new <see cref="KeyboardEvent"/> from a <see cref="KeyboardState"/>.
        /// </summary>
        /// <param name="state">The MonoGame representation of the keyboard.</param>
        public KeyboardEvent(KeyboardState state)
        {
            CapsLock = state.CapsLock;
            NumLock = state.NumLock;
            PressedKeys = state.GetPressedKeys();
        }

        /// <summary>
        /// Constructs a new <see cref="KeyboardEvent"/> from an existing <see cref="KeyboardEvent"/>.
        /// </summary>
        /// <param name="keyboardEvent">The existing keyboard event.</param>
        public KeyboardEvent(KeyboardEvent keyboardEvent)
        {
            CapsLock = keyboardEvent.CapsLock;
            NumLock = keyboardEvent.NumLock;
            PressedKeys = keyboardEvent.PressedKeys;
        }

        /// <summary>
        /// Clones the current keyboard event instance and returns a copy.
        /// </summary>
        /// <returns>A copy of the current keyboard event.</returns>
        public virtual KeyboardEvent Clone()
        {
            return new KeyboardEvent(this);
        }

        [OnDeserialized]
        internal void OnDeserialize(StreamingContext context)
        {
            _pressedKeysHashSet = new HashSet<Keys>(_pressedKeysArray);
        }

        /// <summary>
        /// Whether or not Caps Lock is currently active.
        /// </summary>
        public bool CapsLock { get; set; }

        /// <summary>
        /// Whether or not Num Lock is currently active.
        /// </summary>
        public bool NumLock { get; set; }

        /// <summary>
        /// A list of keys that are currently being pressed.
        /// </summary>
        public Keys[] PressedKeys
        {
            get
            {
                return _pressedKeysArray;
            }
            set
            {
                _pressedKeysArray = value;
                _pressedKeysHashSet = new HashSet<Keys>(_pressedKeysArray);
            }
        }

        /// <summary>
        /// Returns the state of the specified key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>The state of the key.</returns>
        public KeyState this[Keys key]
        {
            get { return IsKeyDown(key) ? KeyState.Down : KeyState.Up; }
        }

        /// <summary>
        /// Returns whether a key is currently being pressed down or held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is pressed, false otherwise.</returns>
        public bool IsKeyDown(Keys key)
        {
            return _pressedKeysHashSet.Contains(key);
        }

        /// <summary>
        /// Returns whether a key is not currently pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is not pressed, false otherwise.</returns>
        public bool IsKeyUp(Keys key)
        {
            return !_pressedKeysHashSet.Contains(key);
        }
    }
}