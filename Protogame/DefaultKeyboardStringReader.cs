/*
    Copyright (C) 2011 by Hunter Haydel

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class DefaultKeyboardStringReader : IKeyboardStringReader
    {
        #region Key Maps
        // Not super happy about having to do this, but it was either this or a giant switch statement and this kept
        // the method cleaner.
        private static Dictionary<Keys, Tuple<char, char>> alphabetKeyMap = new Dictionary<Keys, Tuple<char, char>>
        {
            {Keys.Q, new Tuple<char, char>('q', 'Q')},
            {Keys.W,  new Tuple<char, char>('w', 'W')},
            {Keys.E,  new Tuple<char, char>('e', 'E')},
            {Keys.R,  new Tuple<char, char>('r', 'R')},
            {Keys.T,  new Tuple<char, char>('t', 'T')},
            {Keys.Y,  new Tuple<char, char>('y', 'Y')},
            {Keys.U,  new Tuple<char, char>('u', 'U')},
            {Keys.I,  new Tuple<char, char>('i', 'I')},
            {Keys.O,  new Tuple<char, char>('o', 'O')},
            {Keys.P,  new Tuple<char, char>('p', 'P')},

            {Keys.A, new Tuple<char, char>('a', 'A')},
            {Keys.S,  new Tuple<char, char>('s', 'S')},
            {Keys.D,  new Tuple<char, char>('d', 'D')},
            {Keys.F,  new Tuple<char, char>('f', 'F')},
            {Keys.G,  new Tuple<char, char>('g', 'G')},
            {Keys.H,  new Tuple<char, char>('h', 'H')},
            {Keys.J,  new Tuple<char, char>('j', 'J')},
            {Keys.K,  new Tuple<char, char>('k', 'K')},
            {Keys.L,  new Tuple<char, char>('l', 'L')},

            {Keys.Z, new Tuple<char, char>('z', 'Z')},
            {Keys.X,  new Tuple<char, char>('x', 'X')},
            {Keys.C,  new Tuple<char, char>('c', 'C')},
            {Keys.V,  new Tuple<char, char>('v', 'V')},
            {Keys.B,  new Tuple<char, char>('b', 'B')},
            {Keys.N,  new Tuple<char, char>('n', 'N')},
            {Keys.M,  new Tuple<char, char>('m', 'M')}
        };

        private static Dictionary<Keys, Tuple<char, char>> symbolNumberKeyMap = new Dictionary<Keys, Tuple<char, char>>
        {
            {Keys.OemTilde, new Tuple<char, char>('`', '~')},
            {Keys.D1,  new Tuple<char, char>('1', '!')},
            {Keys.D2,  new Tuple<char, char>('2', '@')},
            {Keys.D3,  new Tuple<char, char>('3', '#')},
            {Keys.D4,  new Tuple<char, char>('4', '$')},
            {Keys.D5,  new Tuple<char, char>('5', '%')},
            {Keys.D6,  new Tuple<char, char>('6', '^')},
            {Keys.D7,  new Tuple<char, char>('7', '&')},
            {Keys.D8,  new Tuple<char, char>('8', '*')},
            {Keys.D9,  new Tuple<char, char>('9', '(')},
            {Keys.D0,  new Tuple<char, char>('0', ')')},
            {Keys.OemMinus,  new Tuple<char, char>('-', '_')},
            {Keys.OemPlus,  new Tuple<char, char>('+', '=')},

            {Keys.OemOpenBrackets,  new Tuple<char, char>('[', '{')},
            {Keys.OemCloseBrackets,  new Tuple<char, char>(']', '}')},
            {Keys.OemPipe,  new Tuple<char, char>('\\', '|')},

            {Keys.OemSemicolon,  new Tuple<char, char>(';', ':')},
            {Keys.OemQuotes,  new Tuple<char, char>('\'', '"')},

            {Keys.OemComma,  new Tuple<char, char>(',', '<')},
            {Keys.OemPeriod,  new Tuple<char, char>('.', '>')},
            {Keys.OemQuestion,  new Tuple<char, char>('/', '?')},

            {Keys.Space, new Tuple<char, char>(' ', ' ')}
        };

        private static Dictionary<Keys, char> numPadKeyMap = new Dictionary<Keys, char>
        {
            {Keys.NumPad0, '0'},
            {Keys.NumPad1, '1'},
            {Keys.NumPad2, '2'},
            {Keys.NumPad3, '3'},
            {Keys.NumPad4, '4'},
            {Keys.NumPad5, '5'},
            {Keys.NumPad6, '6'},
            {Keys.NumPad7, '7'},
            {Keys.NumPad8, '8'},
            {Keys.NumPad9, '9'},
            {Keys.Decimal, '.'}
        };

        private static Dictionary<Keys, char> numPadMathKeyMap = new Dictionary<Keys, char>
        {
            {Keys.Divide, '/'},
            {Keys.Multiply, '*'},
            {Keys.Subtract, '-'},
            {Keys.Add, '+'}
        };

        #endregion

        #region Private Members

        // Dictionary to keep track of when each pressed key should repeat.
        private Dictionary<Keys, double> pressedKeys;

        #endregion

        #region Properties

        /// <summary>
        /// The amount of time in milliseconds a key needs to be held down to repeat for
        /// the first time.
        /// </summary>
        public int FirstRepeatKeyInterval
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of time in milliseconds a key needs to be held down to repeat for
        /// the second time and beyond.
        /// </summary>
        public int RepeatKeyInterval
        {
            get;
            set;
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the KeyboardStringBuilder class.
        /// </summary>
        public DefaultKeyboardStringReader()
        {
            this.FirstRepeatKeyInterval = 450;
            this.RepeatKeyInterval = 50;
            this.pressedKeys = new Dictionary<Keys, double>();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Process the current keyboard state and add or remove characters from the given StringBuilder.
        /// </summary>
        /// <param name="keyboard">The keyboard state input is being read from.</param>
        /// <param name="time">Current GameTime</param>
        /// <param name="text">The StringBuilder to be modified based on keyboard state.</param>
        public void Process(KeyboardState keyboard, GameTime time, StringBuilder text)
        {

            var keys = keyboard.GetPressedKeys();

            // check and see if shift is down, caps lock is on, and/or num lock is on
            var shift = (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift));

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
            var capsLock = System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock);
            var numLock = System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.NumLock);
#else
			var capsLock = false;
			var numLock = false;
#endif

            // remove any keys that aren't down anymore from pressed keys
            foreach (Keys key in this.pressedKeys.Keys.Except(keys).ToArray())
            {
                this.pressedKeys.Remove(key);
            }

            foreach (Keys key in keys)
            {
                // if the key wasn't pressed the last go round process it and set the repeat time to the
                // current time + the first repeat interval.  Otherwise process it and set the repeat time
                // to the current time + the repeat key interval.

                if (!this.pressedKeys.Keys.Any(k => k == key))
                {
                    ProcessKey(key, shift, capsLock, numLock, text);
                    this.pressedKeys[key] = time.TotalGameTime.TotalMilliseconds + this.FirstRepeatKeyInterval;
                }
                else if (time.TotalGameTime.TotalMilliseconds > this.pressedKeys[key])
                {
                    ProcessKey(key, shift, capsLock, numLock, text);
                    this.pressedKeys[key] = time.TotalGameTime.TotalMilliseconds + this.RepeatKeyInterval;
                }
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Modifies the StringBuilder based on the pressed key.
        /// </summary>
        /// <param name="key">The key being pressed.</param>
        /// <param name="shift">Is the shift key down or capslock on?</param>
        /// <param name="capsLock">Is caps lock on?</param>
        /// <param name="numLock">Is num lock on?</param>
        /// <param name="text">The StringBuilder to be modified.</param>
        private void ProcessKey(Keys key, bool shift, bool capsLock, bool numLock, StringBuilder text)
        {
            if (key == Keys.Back && text.Length > 0) text = text.Remove(text.Length - 1, 1);

            var newChar = GetCharacter(key, shift, capsLock, numLock);

            if (newChar.HasValue) text.Append(newChar.Value);
        }

        /// <summary>
        /// Gets the character associated with the given key/shift pair.
        /// </summary>
        /// <param name="key">The key being pressed.</param>
        /// <param name="shift">Is the shift key down?</param>
        /// <param name="capsLock">Is caps lock on?</param>
        /// <param name="numLock">Is num lock on?</param>
        /// <returns>The character the key/shift pair maps to.</returns>
        private char? GetCharacter(Keys key, bool shift, bool capsLock, bool numLock)
        {
            var newChar = new char?();
            if (alphabetKeyMap.Keys.Contains(key))
            {
                var characterMap = alphabetKeyMap[key];
                newChar = (shift ^ capsLock) ? characterMap.Item2 : characterMap.Item1;
            }
            else if (symbolNumberKeyMap.Keys.Contains(key))
            {
                var characterMap = symbolNumberKeyMap[key];
                newChar = (shift) ? characterMap.Item2 : characterMap.Item1;
            }
            else if (numPadKeyMap.ContainsKey(key))
            {
                if (numLock) newChar = numPadKeyMap[key];
            }
            else if (numPadMathKeyMap.ContainsKey(key))
            {
                newChar = numPadMathKeyMap[key];
            }

            return newChar;
        }

        #endregion
    }
}
