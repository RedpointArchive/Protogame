using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Protogame
{
    public static class MouseStateExtensions
    {
        private static Dictionary<int, ButtonState> m_LeftState = new Dictionary<int, ButtonState>();
        private static Dictionary<int, ButtonState> m_MiddleState = new Dictionary<int, ButtonState>();
        private static Dictionary<int, ButtonState> m_RightState = new Dictionary<int, ButtonState>();

        public static bool LeftPressed(this MouseState state, object obj)
        {
            return LeftPressed(state, obj == null ? 0 : obj.GetHashCode());
        }

        public static bool MiddlePressed(this MouseState state, object obj)
        {
            return MiddlePressed(state, obj == null ? 0 : obj.GetHashCode());
        }

        public static bool RightPressed(this MouseState state, object obj)
        {
            return RightPressed(state, obj == null ? 0 : obj.GetHashCode());
        }

        public static bool LeftPressed(this MouseState state, int hash)
        {
            if (!m_LeftState.ContainsKey(hash)) m_LeftState[hash] = ButtonState.Released;
            var result = (m_LeftState[hash] == ButtonState.Released &&
                state.LeftButton == ButtonState.Pressed);
            m_LeftState[hash] = state.LeftButton;
            return result;
        }

        public static bool MiddlePressed(this MouseState state, int hash)
        {
            if (!m_MiddleState.ContainsKey(hash)) m_MiddleState[hash] = ButtonState.Released;
            var result = (m_MiddleState[hash] == ButtonState.Released &&
                state.MiddleButton == ButtonState.Pressed);
            m_MiddleState[hash] = state.MiddleButton;
            return result;
        }

        public static bool RightPressed(this MouseState state, int hash)
        {
            if (!m_RightState.ContainsKey(hash)) m_RightState[hash] = ButtonState.Released;
            var result = (m_RightState[hash] == ButtonState.Released &&
                state.RightButton == ButtonState.Pressed);
            m_RightState[hash] = state.RightButton;
            return result;
        }
    }
}

