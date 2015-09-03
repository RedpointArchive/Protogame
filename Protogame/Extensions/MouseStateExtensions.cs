namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Extensions for the XNA <see cref="MouseState"/> class.
    /// <para>
    /// This provides state-aware functions for detecting changes in the mouse state, without having
    /// to track the previous state in each object that wants to know about button presses.
    /// </para>
    /// </summary>
    /// <module>Extensions</module>
    public static class MouseStateExtensions
    {
        /// <summary>
        /// The dictionary that keeps track of the left mouse button state per object.  The hash code of the
        /// object doing the check is used as the key, so that checks on one object will not interfere with
        /// another object.
        /// </summary>
        private static readonly Dictionary<int, ButtonState> m_LeftState = new Dictionary<int, ButtonState>();

        /// <summary>
        /// The dictionary that keeps track of the middle mouse button state per object.  The hash code of the
        /// object doing the check is used as the key, so that checks on one object will not interfere with
        /// another object.
        /// </summary>
        private static readonly Dictionary<int, ButtonState> m_MiddleState = new Dictionary<int, ButtonState>();

        /// <summary>
        /// The dictionary that keeps track of the right mouse button state per object.  The hash code of the
        /// object doing the check is used as the key, so that checks on one object will not interfere with
        /// another object.
        /// </summary>
        private static readonly Dictionary<int, ButtonState> m_RightState = new Dictionary<int, ButtonState>();

        /// <summary>
        /// Detects whether or not the left mouse button has changed state.
        /// </summary>
        /// <param name="state">The mouse state to check.</param>
        /// <param name="obj">The object for the unique check to be done against.</param>
        /// <returns>The state change of the left mouse button, or null if there's no change.</returns>
        public static ButtonState? LeftChanged(this MouseState state, object obj)
        {
            return LeftChanged(state, obj == null ? 0 : obj.GetHashCode());
        }

        /// <summary>
        /// Detects whether or not the left mouse button has changed state.
        /// </summary>
        /// <param name="state">The mouse state to check.</param>
        /// <param name="hash">The unique hash code for the unique check to be done against.</param>
        /// <returns>The state change of the left mouse button, or null if there's no change.</returns>
        public static ButtonState? LeftChanged(this MouseState state, int hash)
        {
            if (!m_LeftState.ContainsKey(hash))
            {
                m_LeftState[hash] = ButtonState.Released;
            }

            var wasPressed = m_LeftState[hash] == ButtonState.Released && state.LeftButton == ButtonState.Pressed;
            var wasReleased = m_LeftState[hash] == ButtonState.Pressed && state.LeftButton == ButtonState.Released;
            m_LeftState[hash] = state.LeftButton;
            return wasPressed ? ButtonState.Pressed : wasReleased ? (ButtonState?)ButtonState.Released : null;
        }

        /// <summary>
        /// Detects whether or not the middle mouse button has changed state.
        /// </summary>
        /// <param name="state">The mouse state to check.</param>
        /// <param name="obj">The object for the unique check to be done against.</param>
        /// <returns>The state change of the middle mouse button, or null if there's no change.</returns>
        public static ButtonState? MiddleChanged(this MouseState state, object obj)
        {
            return MiddleChanged(state, obj == null ? 0 : obj.GetHashCode());
        }

        /// <summary>
        /// Detects whether or not the middle mouse button has changed state.
        /// </summary>
        /// <param name="state">The mouse state to check.</param>
        /// <param name="hash">The unique hash code for the unique check to be done against.</param>
        /// <returns>The state change of the middle mouse button, or null if there's no change.</returns>
        public static ButtonState? MiddleChanged(this MouseState state, int hash)
        {
            if (!m_MiddleState.ContainsKey(hash))
            {
                m_MiddleState[hash] = ButtonState.Released;
            }

            var wasPressed = m_MiddleState[hash] == ButtonState.Released && state.MiddleButton == ButtonState.Pressed;
            var wasReleased = m_MiddleState[hash] == ButtonState.Pressed && state.MiddleButton == ButtonState.Released;
            m_MiddleState[hash] = state.MiddleButton;
            return wasPressed ? ButtonState.Pressed : wasReleased ? (ButtonState?)ButtonState.Released : null;
        }

        /// <summary>
        /// Detects whether or not the right mouse button has changed state.
        /// </summary>
        /// <param name="state">The mouse state to check.</param>
        /// <param name="obj">The object for the unique check to be done against.</param>
        /// <returns>The state change of the right mouse button, or null if there's no change.</returns>
        public static ButtonState? RightChanged(this MouseState state, object obj)
        {
            return RightChanged(state, obj == null ? 0 : obj.GetHashCode());
        }

        /// <summary>
        /// Detects whether or not the right mouse button has changed state.
        /// </summary>
        /// <param name="state">The mouse state to check.</param>
        /// <param name="hash">The unique hash code for the unique check to be done against.</param>
        /// <returns>The state change of the right mouse button, or null if there's no change.</returns>
        public static ButtonState? RightChanged(this MouseState state, int hash)
        {
            if (!m_RightState.ContainsKey(hash))
            {
                m_RightState[hash] = ButtonState.Released;
            }

            var wasPressed = m_RightState[hash] == ButtonState.Released && state.RightButton == ButtonState.Pressed;
            var wasReleased = m_RightState[hash] == ButtonState.Pressed && state.RightButton == ButtonState.Released;
            m_RightState[hash] = state.RightButton;
            return wasPressed ? ButtonState.Pressed : wasReleased ? (ButtonState?)ButtonState.Released : null;
        }

        #region Legacy Methods

        // ReSharper disable once CSharpWarnings::CS1591
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        [Obsolete("Use LeftChanged instead", true)]
        public static bool LeftPressed(this MouseState state, object obj)
        {
            return LeftPressed(state, obj == null ? 0 : obj.GetHashCode());
        }

        // ReSharper disable once CSharpWarnings::CS1591
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        [Obsolete("Use LeftChanged instead", true)]
        public static bool LeftPressed(this MouseState state, int hash)
        {
            if (!m_LeftState.ContainsKey(hash))
            {
                m_LeftState[hash] = ButtonState.Released;
            }

            var result = m_LeftState[hash] == ButtonState.Released && state.LeftButton == ButtonState.Pressed;
            m_LeftState[hash] = state.LeftButton;
            return result;
        }

        // ReSharper disable once CSharpWarnings::CS1591
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        [Obsolete("Use MiddleChanged instead", true)]
        public static bool MiddlePressed(this MouseState state, object obj)
        {
            return MiddlePressed(state, obj == null ? 0 : obj.GetHashCode());
        }

        // ReSharper disable once CSharpWarnings::CS1591
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        [Obsolete("Use MiddleChanged instead", true)]
        public static bool MiddlePressed(this MouseState state, int hash)
        {
            if (!m_MiddleState.ContainsKey(hash))
            {
                m_MiddleState[hash] = ButtonState.Released;
            }

            var result = m_MiddleState[hash] == ButtonState.Released && state.MiddleButton == ButtonState.Pressed;
            m_MiddleState[hash] = state.MiddleButton;
            return result;
        }

        // ReSharper disable once CSharpWarnings::CS1591
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        [Obsolete("Use RightChanged instead", true)]
        public static bool RightPressed(this MouseState state, object obj)
        {
            return RightPressed(state, obj == null ? 0 : obj.GetHashCode());
        }

        // ReSharper disable once CSharpWarnings::CS1591
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        [Obsolete("Use RightChanged instead", true)]
        public static bool RightPressed(this MouseState state, int hash)
        {
            if (!m_RightState.ContainsKey(hash))
            {
                m_RightState[hash] = ButtonState.Released;
            }

            var result = m_RightState[hash] == ButtonState.Released && state.RightButton == ButtonState.Pressed;
            m_RightState[hash] = state.RightButton;
            return result;
        }

        #endregion
    }
}