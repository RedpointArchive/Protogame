namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Extensions for the XNA <see cref="KeyboardState"/> class.
    /// <para>
    /// This provides state-aware functions for detecting changes in the keyboard state, without having
    /// to track the previous state in each object that wants to know about key presses.
    /// </para>
    /// </summary>
    /// <module>Extensions</module>
    public static class KeyboardStateExtensions
    {
        /// <summary>
        /// The m_ global key state.
        /// </summary>
        private static Dictionary<int, Dictionary<Keys, KeyState>> m_GlobalKeyState;

        /// <summary>
        /// Detects whether the specified key has changed state.
        /// </summary>
        /// <param name="state">The keyboard state to check.</param>
        /// <param name="obj">The object for the unique check to be done against.</param>
        /// <param name="key">The key to check.</param>
        /// <returns>The state change of the key, or null if there's no change.</returns>
        public static KeyState? IsKeyChanged(this KeyboardState state, object obj, Keys key)
        {
            if (m_GlobalKeyState == null)
            {
                m_GlobalKeyState = new Dictionary<int, Dictionary<Keys, KeyState>>();
            }

            var hash = obj == null ? 0 : obj.GetHashCode();
            if (!m_GlobalKeyState.ContainsKey(hash))
            {
                m_GlobalKeyState[hash] = new Dictionary<Keys, KeyState>();
            }

            var m_KeyState = m_GlobalKeyState[hash];
            var oldPressed = KeyState.Up;
            var newPressed = state[key];
            KeyState? result = null;
            if (m_KeyState.ContainsKey(key))
            {
                oldPressed = m_KeyState[key];
            }

            if (oldPressed == KeyState.Up && newPressed == KeyState.Down)
            {
                result = KeyState.Down;
            }
            else if (oldPressed == KeyState.Down && newPressed == KeyState.Up)
            {
                result = KeyState.Up;
            }

            m_KeyState[key] = state[key];
            return result;
        }

        // ReSharper disable once CSharpWarnings::CS1591
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        [Obsolete("Use IsKeyChanged instead", true)]
        public static bool IsKeyPressed(this KeyboardState state, object obj, Keys key)
        {
            if (m_GlobalKeyState == null)
            {
                m_GlobalKeyState = new Dictionary<int, Dictionary<Keys, KeyState>>();
            }

            var hash = obj == null ? 0 : obj.GetHashCode();
            if (!m_GlobalKeyState.ContainsKey(hash))
            {
                m_GlobalKeyState[hash] = new Dictionary<Keys, KeyState>();
            }

            var m_KeyState = m_GlobalKeyState[hash];
            var oldPressed = KeyState.Up;
            var newPressed = state[key];
            var result = false;
            if (m_KeyState.ContainsKey(key))
            {
                oldPressed = m_KeyState[key];
            }

            if (oldPressed == KeyState.Up && newPressed == KeyState.Down)
            {
                result = true;
            }

            m_KeyState[key] = state[key];
            return result;
        }

        /// <summary>
        /// Tries to convert keyboard input to characters and prevents repeatedly returning the
        /// same character if a key was pressed last frame, but not yet unpressed this frame.
        /// </summary>
        /// <param name="keyboard">
        /// The current KeyboardState.
        /// </param>
        /// <param name="oldKeyboard">
        /// The KeyboardState of the previous frame.
        /// </param>
        /// <param name="key">
        /// When this method returns, contains the correct character if conversion succeeded.
        /// Else contains the null, (000), character.
        /// </param>
        /// <param name="special">
        /// The special.
        /// </param>
        /// <returns>
        /// True if conversion was successful.
        /// </returns>
        public static bool TryConvertKeyboardInput(
            this KeyboardState keyboard, 
            KeyboardState oldKeyboard, 
            out char key, 
            out Keys special)
        {
            var keys = keyboard.GetPressedKeys();
            var shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            special = Keys.None;

            if (keys.Length > 0 && !oldKeyboard.IsKeyDown(keys[0]))
            {
                switch (keys[0])
                {
                        // Alphabet keys
                    case Keys.A:
                        if (shift)
                        {
                            key = 'A';
                        }
                        else
                        {
                            key = 'a';
                        }

                        return true;
                    case Keys.B:
                        if (shift)
                        {
                            key = 'B';
                        }
                        else
                        {
                            key = 'b';
                        }

                        return true;
                    case Keys.C:
                        if (shift)
                        {
                            key = 'C';
                        }
                        else
                        {
                            key = 'c';
                        }

                        return true;
                    case Keys.D:
                        if (shift)
                        {
                            key = 'D';
                        }
                        else
                        {
                            key = 'd';
                        }

                        return true;
                    case Keys.E:
                        if (shift)
                        {
                            key = 'E';
                        }
                        else
                        {
                            key = 'e';
                        }

                        return true;
                    case Keys.F:
                        if (shift)
                        {
                            key = 'F';
                        }
                        else
                        {
                            key = 'f';
                        }

                        return true;
                    case Keys.G:
                        if (shift)
                        {
                            key = 'G';
                        }
                        else
                        {
                            key = 'g';
                        }

                        return true;
                    case Keys.H:
                        if (shift)
                        {
                            key = 'H';
                        }
                        else
                        {
                            key = 'h';
                        }

                        return true;
                    case Keys.I:
                        if (shift)
                        {
                            key = 'I';
                        }
                        else
                        {
                            key = 'i';
                        }

                        return true;
                    case Keys.J:
                        if (shift)
                        {
                            key = 'J';
                        }
                        else
                        {
                            key = 'j';
                        }

                        return true;
                    case Keys.K:
                        if (shift)
                        {
                            key = 'K';
                        }
                        else
                        {
                            key = 'k';
                        }

                        return true;
                    case Keys.L:
                        if (shift)
                        {
                            key = 'L';
                        }
                        else
                        {
                            key = 'l';
                        }

                        return true;
                    case Keys.M:
                        if (shift)
                        {
                            key = 'M';
                        }
                        else
                        {
                            key = 'm';
                        }

                        return true;
                    case Keys.N:
                        if (shift)
                        {
                            key = 'N';
                        }
                        else
                        {
                            key = 'n';
                        }

                        return true;
                    case Keys.O:
                        if (shift)
                        {
                            key = 'O';
                        }
                        else
                        {
                            key = 'o';
                        }

                        return true;
                    case Keys.P:
                        if (shift)
                        {
                            key = 'P';
                        }
                        else
                        {
                            key = 'p';
                        }

                        return true;
                    case Keys.Q:
                        if (shift)
                        {
                            key = 'Q';
                        }
                        else
                        {
                            key = 'q';
                        }

                        return true;
                    case Keys.R:
                        if (shift)
                        {
                            key = 'R';
                        }
                        else
                        {
                            key = 'r';
                        }

                        return true;
                    case Keys.S:
                        if (shift)
                        {
                            key = 'S';
                        }
                        else
                        {
                            key = 's';
                        }

                        return true;
                    case Keys.T:
                        if (shift)
                        {
                            key = 'T';
                        }
                        else
                        {
                            key = 't';
                        }

                        return true;
                    case Keys.U:
                        if (shift)
                        {
                            key = 'U';
                        }
                        else
                        {
                            key = 'u';
                        }

                        return true;
                    case Keys.V:
                        if (shift)
                        {
                            key = 'V';
                        }
                        else
                        {
                            key = 'v';
                        }

                        return true;
                    case Keys.W:
                        if (shift)
                        {
                            key = 'W';
                        }
                        else
                        {
                            key = 'w';
                        }

                        return true;
                    case Keys.X:
                        if (shift)
                        {
                            key = 'X';
                        }
                        else
                        {
                            key = 'x';
                        }

                        return true;
                    case Keys.Y:
                        if (shift)
                        {
                            key = 'Y';
                        }
                        else
                        {
                            key = 'y';
                        }

                        return true;
                    case Keys.Z:
                        if (shift)
                        {
                            key = 'Z';
                        }
                        else
                        {
                            key = 'z';
                        }

                        return true;

                        // Decimal keys
                    case Keys.D0:
                        if (shift)
                        {
                            key = ')';
                        }
                        else
                        {
                            key = '0';
                        }

                        return true;
                    case Keys.D1:
                        if (shift)
                        {
                            key = '!';
                        }
                        else
                        {
                            key = '1';
                        }

                        return true;
                    case Keys.D2:
                        if (shift)
                        {
                            key = '@';
                        }
                        else
                        {
                            key = '2';
                        }

                        return true;
                    case Keys.D3:
                        if (shift)
                        {
                            key = '#';
                        }
                        else
                        {
                            key = '3';
                        }

                        return true;
                    case Keys.D4:
                        if (shift)
                        {
                            key = '$';
                        }
                        else
                        {
                            key = '4';
                        }

                        return true;
                    case Keys.D5:
                        if (shift)
                        {
                            key = '%';
                        }
                        else
                        {
                            key = '5';
                        }

                        return true;
                    case Keys.D6:
                        if (shift)
                        {
                            key = '^';
                        }
                        else
                        {
                            key = '6';
                        }

                        return true;
                    case Keys.D7:
                        if (shift)
                        {
                            key = '&';
                        }
                        else
                        {
                            key = '7';
                        }

                        return true;
                    case Keys.D8:
                        if (shift)
                        {
                            key = '*';
                        }
                        else
                        {
                            key = '8';
                        }

                        return true;
                    case Keys.D9:
                        if (shift)
                        {
                            key = '(';
                        }
                        else
                        {
                            key = '9';
                        }

                        return true;

                        // Decimal numpad keys
                    case Keys.NumPad0:
                        key = '0';
                        return true;
                    case Keys.NumPad1:
                        key = '1';
                        return true;
                    case Keys.NumPad2:
                        key = '2';
                        return true;
                    case Keys.NumPad3:
                        key = '3';
                        return true;
                    case Keys.NumPad4:
                        key = '4';
                        return true;
                    case Keys.NumPad5:
                        key = '5';
                        return true;
                    case Keys.NumPad6:
                        key = '6';
                        return true;
                    case Keys.NumPad7:
                        key = '7';
                        return true;
                    case Keys.NumPad8:
                        key = '8';
                        return true;
                    case Keys.NumPad9:
                        key = '9';
                        return true;

                        // Special keys
                    case Keys.OemTilde:
                        if (shift)
                        {
                            key = '~';
                        }
                        else
                        {
                            key = '`';
                        }

                        return true;
                    case Keys.OemSemicolon:
                        if (shift)
                        {
                            key = ':';
                        }
                        else
                        {
                            key = ';';
                        }

                        return true;
                    case Keys.OemQuotes:
                        if (shift)
                        {
                            key = '"';
                        }
                        else
                        {
                            key = '\'';
                        }

                        return true;
                    case Keys.OemQuestion:
                        if (shift)
                        {
                            key = '?';
                        }
                        else
                        {
                            key = '/';
                        }

                        return true;
                    case Keys.OemPlus:
                        if (shift)
                        {
                            key = '+';
                        }
                        else
                        {
                            key = '=';
                        }

                        return true;
                    case Keys.OemPipe:
                        if (shift)
                        {
                            key = '|';
                        }
                        else
                        {
                            key = '\\';
                        }

                        return true;
                    case Keys.OemPeriod:
                        if (shift)
                        {
                            key = '>';
                        }
                        else
                        {
                            key = '.';
                        }

                        return true;
                    case Keys.OemOpenBrackets:
                        if (shift)
                        {
                            key = '{';
                        }
                        else
                        {
                            key = '[';
                        }

                        return true;
                    case Keys.OemCloseBrackets:
                        if (shift)
                        {
                            key = '}';
                        }
                        else
                        {
                            key = ']';
                        }

                        return true;
                    case Keys.OemMinus:
                        if (shift)
                        {
                            key = '_';
                        }
                        else
                        {
                            key = '-';
                        }

                        return true;
                    case Keys.OemComma:
                        if (shift)
                        {
                            key = '<';
                        }
                        else
                        {
                            key = ',';
                        }

                        return true;
                    case Keys.Space:
                        key = ' ';
                        return true;

                        // Special special keys
                    case Keys.Delete:
                        key = (char)0;
                        special = Keys.Delete;
                        return true;
                    case Keys.Back:
                        key = (char)0;
                        special = Keys.Back;
                        return true;
                }
            }

            key = (char)0;
            return false;
        }
    }
}