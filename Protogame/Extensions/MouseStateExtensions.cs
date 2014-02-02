namespace Protogame
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The mouse state extensions.
    /// </summary>
    public static class MouseStateExtensions
    {
        /// <summary>
        /// The m_ left state.
        /// </summary>
        private static readonly Dictionary<int, ButtonState> m_LeftState = new Dictionary<int, ButtonState>();

        /// <summary>
        /// The m_ middle state.
        /// </summary>
        private static readonly Dictionary<int, ButtonState> m_MiddleState = new Dictionary<int, ButtonState>();

        /// <summary>
        /// The m_ right state.
        /// </summary>
        private static readonly Dictionary<int, ButtonState> m_RightState = new Dictionary<int, ButtonState>();

        /// <summary>
        /// The left pressed.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool LeftPressed(this MouseState state, object obj)
        {
            return LeftPressed(state, obj == null ? 0 : obj.GetHashCode());
        }

        /// <summary>
        /// The left pressed.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

        /// <summary>
        /// The middle pressed.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool MiddlePressed(this MouseState state, object obj)
        {
            return MiddlePressed(state, obj == null ? 0 : obj.GetHashCode());
        }

        /// <summary>
        /// The middle pressed.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

        /// <summary>
        /// The right pressed.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool RightPressed(this MouseState state, object obj)
        {
            return RightPressed(state, obj == null ? 0 : obj.GetHashCode());
        }

        /// <summary>
        /// The right pressed.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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
    }
}