using Microsoft.Xna.Framework.Input;
using System;

namespace Protogame
{
    /// <summary>
    /// Represents movement of the mouse cursor on the screen
    /// </summary>
    /// <module>Events</module>
    [Serializable]
    public class MouseMoveEvent : MouseEvent
    {
        /// <summary>
        /// Constructs a new default <see cref="MouseMoveEvent"/>.
        /// </summary>
        public MouseMoveEvent()
        {
        }

        /// <summary>
        /// Constructs a new <see cref="MouseMoveEvent"/> from a <see cref="MouseState"/>.
        /// </summary>
        /// <param name="state">The MonoGame representation of the mouse.</param>
        public MouseMoveEvent(MouseState state) : base(state)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="MouseMoveEvent"/> from an existing <see cref="MouseMoveEvent"/>.
        /// </summary>
        /// <param name="mouseEvent">The existing mouse move event.</param>
        public MouseMoveEvent(MouseMoveEvent mouseEvent) : base(mouseEvent)
        {
            LastX = mouseEvent.LastX;
            LastY = mouseEvent.LastY;
        }

        /// <summary>
        /// Clones the current mouse event instance and returns a copy.
        /// </summary>
        /// <returns>A copy of the current mouse event.</returns>
        public override MouseEvent Clone()
        {
            return new MouseMoveEvent(this);
        }

        /// <summary>
        /// The previous horizontal position of the mouse cursor in relation to the window.
        /// </summary>
        public int LastX { get; set; }

        /// <summary>
        /// The previous vertical position of the mouse cursor in relation to the window.
        /// </summary>
        public int LastY { get; set; }
    }
}