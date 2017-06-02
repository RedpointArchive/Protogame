using Microsoft.Xna.Framework.Input;
using System;

namespace Protogame
{
    /// <summary>
    /// Represents a button on a mouse being pressed down.
    /// </summary>
    /// <module>Events</module>
    [Serializable]
    public class MousePressEvent : MouseEvent
    {
        /// <summary>
        /// Constructs a new default <see cref="MousePressEvent"/>.
        /// </summary>
        public MousePressEvent()
        {
        }

        /// <summary>
        /// Constructs a new <see cref="MousePressEvent"/> from a <see cref="MouseState"/>.
        /// </summary>
        /// <param name="state">The MonoGame representation of the mouse.</param>
        public MousePressEvent(MouseState state) : base(state)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="MousePressEvent"/> from an existing <see cref="MousePressEvent"/>.
        /// </summary>
        /// <param name="mouseEvent">The existing mouse press event.</param>
        public MousePressEvent(MousePressEvent mouseEvent) : base(mouseEvent)
        {
            Button = mouseEvent.Button;
        }

        /// <summary>
        /// Clones the current mouse event instance and returns a copy.
        /// </summary>
        /// <returns>A copy of the current mouse event.</returns>
        public override MouseEvent Clone()
        {
            return new MousePressEvent(this);
        }
        
        /// <summary>
        /// The button that was pressed down.
        /// </summary>
        public MouseButton Button { get; set; }
    }
}