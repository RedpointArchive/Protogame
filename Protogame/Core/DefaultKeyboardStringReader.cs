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

// ReSharper disable CheckNamespace

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
using System.Windows.Forms;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

#pragma warning disable 1591

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IKeyboardStringReader"/>.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IKeyboardStringReader</interface_ref>
    public class DefaultKeyboardStringReader : IKeyboardStringReader
    {
        // Not super happy about having to do this, but it was either this or a giant switch statement and this kept
        // the method cleaner.
        /// <summary>
        /// The alphabet key map.
        /// </summary>
        private static readonly Dictionary<Keys, Tuple<char, char>> AlphabetKeyMap =
            new Dictionary<Keys, Tuple<char, char>>
            {
                { Keys.Q, new Tuple<char, char>('q', 'Q') }, 
                { Keys.W, new Tuple<char, char>('w', 'W') }, 
                { Keys.E, new Tuple<char, char>('e', 'E') }, 
                { Keys.R, new Tuple<char, char>('r', 'R') }, 
                { Keys.T, new Tuple<char, char>('t', 'T') }, 
                { Keys.Y, new Tuple<char, char>('y', 'Y') }, 
                { Keys.U, new Tuple<char, char>('u', 'U') }, 
                { Keys.I, new Tuple<char, char>('i', 'I') }, 
                { Keys.O, new Tuple<char, char>('o', 'O') }, 
                { Keys.P, new Tuple<char, char>('p', 'P') }, 
                { Keys.A, new Tuple<char, char>('a', 'A') }, 
                { Keys.S, new Tuple<char, char>('s', 'S') }, 
                { Keys.D, new Tuple<char, char>('d', 'D') }, 
                { Keys.F, new Tuple<char, char>('f', 'F') }, 
                { Keys.G, new Tuple<char, char>('g', 'G') }, 
                { Keys.H, new Tuple<char, char>('h', 'H') }, 
                { Keys.J, new Tuple<char, char>('j', 'J') }, 
                { Keys.K, new Tuple<char, char>('k', 'K') }, 
                { Keys.L, new Tuple<char, char>('l', 'L') }, 
                { Keys.Z, new Tuple<char, char>('z', 'Z') }, 
                { Keys.X, new Tuple<char, char>('x', 'X') }, 
                { Keys.C, new Tuple<char, char>('c', 'C') }, 
                { Keys.V, new Tuple<char, char>('v', 'V') }, 
                { Keys.B, new Tuple<char, char>('b', 'B') }, 
                { Keys.N, new Tuple<char, char>('n', 'N') }, 
                { Keys.M, new Tuple<char, char>('m', 'M') }
            };

        /// <summary>
        /// The num pad key map.
        /// </summary>
        private static readonly Dictionary<Keys, char> NumPadKeyMap = new Dictionary<Keys, char>
        {
            { Keys.NumPad0, '0' }, 
            { Keys.NumPad1, '1' }, 
            { Keys.NumPad2, '2' }, 
            { Keys.NumPad3, '3' }, 
            { Keys.NumPad4, '4' }, 
            { Keys.NumPad5, '5' }, 
            { Keys.NumPad6, '6' }, 
            { Keys.NumPad7, '7' }, 
            { Keys.NumPad8, '8' }, 
            { Keys.NumPad9, '9' }, 
            { Keys.Decimal, '.' }
        };

        /// <summary>
        /// The num pad math key map.
        /// </summary>
        private static readonly Dictionary<Keys, char> NumPadMathKeyMap = new Dictionary<Keys, char>
        {
            { Keys.Divide, '/' }, 
            { Keys.Multiply, '*' }, 
            { Keys.Subtract, '-' }, 
            { Keys.Add, '+' }
        };

        /// <summary>
        /// The symbol number key map.
        /// </summary>
        private static readonly Dictionary<Keys, Tuple<char, char>> SymbolNumberKeyMap =
            new Dictionary<Keys, Tuple<char, char>>
            {
                { Keys.OemTilde, new Tuple<char, char>('`', '~') }, 
                { Keys.D1, new Tuple<char, char>('1', '!') }, 
                { Keys.D2, new Tuple<char, char>('2', '@') }, 
                { Keys.D3, new Tuple<char, char>('3', '#') }, 
                { Keys.D4, new Tuple<char, char>('4', '$') }, 
                { Keys.D5, new Tuple<char, char>('5', '%') }, 
                { Keys.D6, new Tuple<char, char>('6', '^') }, 
                { Keys.D7, new Tuple<char, char>('7', '&') }, 
                { Keys.D8, new Tuple<char, char>('8', '*') }, 
                { Keys.D9, new Tuple<char, char>('9', '(') }, 
                { Keys.D0, new Tuple<char, char>('0', ')') }, 
                { Keys.OemMinus, new Tuple<char, char>('-', '_') }, 
                { Keys.OemPlus, new Tuple<char, char>('+', '=') }, 
                { Keys.OemOpenBrackets, new Tuple<char, char>('[', '{') }, 
                { Keys.OemCloseBrackets, new Tuple<char, char>(']', '}') }, 
                { Keys.OemPipe, new Tuple<char, char>('\\', '|') }, 
                { Keys.OemSemicolon, new Tuple<char, char>(';', ':') }, 
                { Keys.OemQuotes, new Tuple<char, char>('\'', '"') }, 
                { Keys.OemComma, new Tuple<char, char>(',', '<') }, 
                { Keys.OemPeriod, new Tuple<char, char>('.', '>') }, 
                { Keys.OemQuestion, new Tuple<char, char>('/', '?') }, 
                { Keys.Space, new Tuple<char, char>(' ', ' ') }
            };

        // Dictionary to keep track of when each pressed key should repeat.
        /// <summary>
        /// The pressed keys.
        /// </summary>
        private readonly Dictionary<Keys, double> _pressedKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultKeyboardStringReader"/> class. 
        /// Initializes a new instance of the KeyboardStringBuilder class.
        /// </summary>
        public DefaultKeyboardStringReader()
        {
            FirstRepeatKeyInterval = 450;
            RepeatKeyInterval = 50;
            _pressedKeys = new Dictionary<Keys, double>();
        }

        /// <summary>
        /// The amount of time in milliseconds a key needs to be held down to repeat for
        /// the first time.
        /// </summary>
        /// <value>
        /// The first repeat key interval.
        /// </value>
        public int FirstRepeatKeyInterval { get; set; }

        /// <summary>
        /// The amount of time in milliseconds a key needs to be held down to repeat for
        /// the second time and beyond.
        /// </summary>
        /// <value>
        /// The repeat key interval.
        /// </value>
        public int RepeatKeyInterval { get; set; }

        /// <summary>
        /// Process the current keyboard state and add or remove characters from the given StringBuilder.
        /// </summary>
        /// <param name="keyboard">
        /// The keyboard state input is being read from.
        /// </param>
        /// <param name="time">
        /// Current GameTime.
        /// </param>
        /// <param name="text">
        /// The StringBuilder to be modified based on keyboard state.
        /// </param>
        public void Process(Keys[] pressedKeys, GameTime time, StringBuilder text)
        {
            var keys = pressedKeys;

            // check and see if shift is down, caps lock is on, and/or num lock is on
            var shift = pressedKeys.Contains(Keys.LeftShift) || pressedKeys.Contains(Keys.RightShift);

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
            var capsLock = Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock);
            var numLock = Control.IsKeyLocked(System.Windows.Forms.Keys.NumLock);
#else
			var capsLock = false;
			var numLock = false;
#endif

            // remove any keys that aren't down anymore from pressed keys
            foreach (var key in _pressedKeys.Keys.Except(keys).ToArray())
            {
                _pressedKeys.Remove(key);
            }

            foreach (var key in keys)
            {
                // if the key wasn't pressed the last go round process it and set the repeat time to the
                // current time + the first repeat interval.  Otherwise process it and set the repeat time
                // to the current time + the repeat key interval.
                if (!_pressedKeys.Keys.Any(k => k == key))
                {
                    ProcessKey(key, shift, capsLock, numLock, text);
                    _pressedKeys[key] = time.TotalGameTime.TotalMilliseconds + FirstRepeatKeyInterval;
                }
                else if (time.TotalGameTime.TotalMilliseconds > _pressedKeys[key])
                {
                    ProcessKey(key, shift, capsLock, numLock, text);
                    _pressedKeys[key] = time.TotalGameTime.TotalMilliseconds + RepeatKeyInterval;
                }
            }
        }

        /// <summary>
        /// Gets the character associated with the given key/shift pair.
        /// </summary>
        /// <param name="key">
        /// The key being pressed.
        /// </param>
        /// <param name="shift">
        /// Is the shift key down?.
        /// </param>
        /// <param name="capsLock">
        /// Is caps lock on?.
        /// </param>
        /// <param name="numLock">
        /// Is num lock on?.
        /// </param>
        /// <returns>
        /// The character the key/shift pair maps to.
        /// </returns>
        private char? GetCharacter(Keys key, bool shift, bool capsLock, bool numLock)
        {
            var newChar = new char?();
            if (AlphabetKeyMap.Keys.Contains(key))
            {
                var characterMap = AlphabetKeyMap[key];
                newChar = (shift ^ capsLock) ? characterMap.Item2 : characterMap.Item1;
            }
            else if (SymbolNumberKeyMap.Keys.Contains(key))
            {
                var characterMap = SymbolNumberKeyMap[key];
                newChar = shift ? characterMap.Item2 : characterMap.Item1;
            }
            else if (NumPadKeyMap.ContainsKey(key))
            {
                if (numLock)
                {
                    newChar = NumPadKeyMap[key];
                }
            }
            else if (NumPadMathKeyMap.ContainsKey(key))
            {
                newChar = NumPadMathKeyMap[key];
            }

            return newChar;
        }

        /// <summary>
        /// Modifies the StringBuilder based on the pressed key.
        /// </summary>
        /// <param name="key">
        /// The key being pressed.
        /// </param>
        /// <param name="shift">
        /// Is the shift key down or capslock on?.
        /// </param>
        /// <param name="capsLock">
        /// Is caps lock on?.
        /// </param>
        /// <param name="numLock">
        /// Is num lock on?.
        /// </param>
        /// <param name="text">
        /// The StringBuilder to be modified.
        /// </param>
        private void ProcessKey(Keys key, bool shift, bool capsLock, bool numLock, StringBuilder text)
        {
            if (key == Keys.Back && text.Length > 0)
            {
                text = text.Remove(text.Length - 1, 1);
            }

            var newChar = GetCharacter(key, shift, capsLock, numLock);

            if (newChar.HasValue)
            {
                text.Append(newChar.Value);
            }
        }
    }
}