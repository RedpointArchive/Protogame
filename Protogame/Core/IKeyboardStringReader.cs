namespace Protogame
{
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    
    public interface IKeyboardStringReader
    {
        int FirstRepeatKeyInterval { get; set; }
        
        int RepeatKeyInterval { get; set; }
        
        void Process(Keys[] pressedKeys, GameTime time, StringBuilder text);
    }
}