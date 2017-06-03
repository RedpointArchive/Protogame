using Microsoft.Xna.Framework.Input;
using System;

namespace Protogame
{
    /// <summary>
    /// Represents the mouse wheel being scrolled.
    /// </summary>
    /// <module>Events</module>
    [Serializable]
    public class MouseScrollEvent : MouseEvent
    {
        /// <summary>
        /// Constructs a new default <see cref="MouseScrollEvent"/>.
        /// </summary>
        public MouseScrollEvent()
        {
        }

        /// <summary>
        /// Constructs a new <see cref="MouseScrollEvent"/> from a <see cref="MouseState"/>.
        /// </summary>
        /// <param name="state">The MonoGame representation of the mouse.</param>
        public MouseScrollEvent(MouseState state) : base(state)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="MouseScrollEvent"/> from an existing <see cref="MouseScrollEvent"/>.
        /// </summary>
        /// <param name="mouseEvent">The existing mouse release event.</param>
        public MouseScrollEvent(MouseScrollEvent mouseEvent) : base(mouseEvent)
        {
            ScrollDelta = mouseEvent.ScrollDelta;
        }

        /// <summary>
        /// Clones the current mouse event instance and returns a copy.
        /// </summary>
        /// <returns>A copy of the current mouse event.</returns>
        public override MouseEvent Clone()
        {
            return new MouseScrollEvent(this);
        }

        /// <summary>
        /// The amount the mouse wheel has scrolled by.
        /// </summary>
        public float ScrollDelta { get; set; }
    }
}