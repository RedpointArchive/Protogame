using Microsoft.Xna.Framework.Input;
using System;

namespace Protogame
{
    /// <summary>
    /// Represents a button on a mouse being released.
    /// </summary>
    /// <module>Events</module>
    [Serializable]
    public class MouseReleaseEvent : MouseEvent
    {
        /// <summary>
        /// Constructs a new default <see cref="MouseReleaseEvent"/>.
        /// </summary>
        public MouseReleaseEvent()
        {
        }

        /// <summary>
        /// Constructs a new <see cref="MouseReleaseEvent"/> from a <see cref="MouseState"/>.
        /// </summary>
        /// <param name="state">The MonoGame representation of the mouse.</param>
        public MouseReleaseEvent(MouseState state) : base(state)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="MouseReleaseEvent"/> from an existing <see cref="MouseReleaseEvent"/>.
        /// </summary>
        /// <param name="mouseEvent">The existing mouse release event.</param>
        public MouseReleaseEvent(MouseReleaseEvent mouseEvent) : base(mouseEvent)
        {
            Button = mouseEvent.Button;
        }

        /// <summary>
        /// Clones the current mouse event instance and returns a copy.
        /// </summary>
        /// <returns>A copy of the current mouse event.</returns>
        public override MouseEvent Clone()
        {
            return new MouseReleaseEvent(this);
        }

        /// <summary>
        /// The mouse button that was released.
        /// </summary>
        public MouseButton Button { get; set; }
    }
}