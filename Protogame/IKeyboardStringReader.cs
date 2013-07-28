using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public interface IKeyboardStringReader
    {
        int FirstRepeatKeyInterval { get; set; }
        int RepeatKeyInterval { get; set; }
        void Process(KeyboardState keyboard, GameTime time, StringBuilder text);
    }
}

