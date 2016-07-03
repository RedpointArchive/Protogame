using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class NullKeyboardStringReader : IKeyboardStringReader
    {
        public int FirstRepeatKeyInterval { get; set; }
        public int RepeatKeyInterval { get; set; }
        public void Process(KeyboardState keyboard, GameTime time, StringBuilder text)
        {
            throw new NotSupportedException();
        }
    }
}