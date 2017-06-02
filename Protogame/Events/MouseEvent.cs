using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.Serialization;

namespace Protogame
{
    /// <summary>
    /// Represents a mouse event.
    /// </summary>
    /// <module>Events</module>
    [Serializable]
    public class MouseEvent : Event
    {
        /// <summary>
        /// Constructs a new default <see cref="MouseEvent"/>.
        /// </summary>
        public MouseEvent()
        {
        }

        /// <summary>
        /// Constructs a new <see cref="MouseEvent"/> from a <see cref="MouseState"/>.
        /// </summary>
        /// <param name="state">The MonoGame representation of the mouse.</param>
        public MouseEvent(MouseState state)
        {
            X = state.X;
            Y = state.Y;
            LeftButton = state.LeftButton;
            MiddleButton = state.MiddleButton;
            RightButton = state.RightButton;
            XButton1 = state.XButton1;
            XButton2 = state.XButton2;
        }

        /// <summary>
        /// Constructs a new <see cref="MouseEvent"/> from an existing <see cref="MouseEvent"/>.
        /// </summary>
        /// <param name="mouseEvent">The existing mouse event.</param>
        public MouseEvent(MouseEvent mouseEvent)
        {
            X = mouseEvent.X;
            Y = mouseEvent.Y;
            LeftButton = mouseEvent.LeftButton;
            MiddleButton = mouseEvent.MiddleButton;
            RightButton = mouseEvent.RightButton;
            XButton1 = mouseEvent.XButton1;
            XButton2 = mouseEvent.XButton2;
        }

        /// <summary>
        /// Clones the current mouse event instance and returns a copy.
        /// </summary>
        /// <returns>A copy of the current mouse event.</returns>
        public virtual MouseEvent Clone()
        {
            return new MouseEvent(this);
        }

        /// <summary>
        /// The horizontal position of the cursor in relation to the window.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The vertical position of the cursor in relation to the window.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The cursor position in relation to the window.
        /// </summary>
        public Point Position => new Point(X, Y);

        /// <summary>
        /// Returns cumulative scroll wheel value since the game start.
        /// </summary>
        public int ScrollWheelValue { get; set; }

        /// <summary>
        /// The state of the left mouse button.
        /// </summary>
        public ButtonState LeftButton { get; set; }

        /// <summary>
        /// The state of the middle mouse button.
        /// </summary>
        public ButtonState MiddleButton { get; set; }

        /// <summary>
        /// The state of the right mouse button.
        /// </summary>
        public ButtonState RightButton { get; set; }

        /// <summary>
        /// The state of extra mouse button 1.
        /// </summary>
        public ButtonState XButton1 { get; set; }

        /// <summary>
        /// The state of extra mouse button 2.
        /// </summary>
        public ButtonState XButton2 { get; set; }

        /// <summary>
        /// The MonoGame mouse state.
        /// </summary>
        /// <remarks>
        /// Calling this property will result in a new <see cref="MouseState"/> being
        /// constructed.  You should use the members directly available on <see cref="MouseEvent"/>
        /// instead of using this property.
        /// </remarks>
        [Obsolete("Use the MouseEvent members directly instead of MouseState.")]
        public MouseState MouseState => new MouseState(X, Y, ScrollWheelValue, LeftButton, MiddleButton, RightButton, XButton1, XButton2);
    }
}