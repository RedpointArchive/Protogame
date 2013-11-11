// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class ConsoleEventListener : IEventListener<IGameContext>
    {
        private readonly IConsole m_Console;
    
        public ConsoleEventListener(
            IConsole console)
        {
            this.m_Console = console;
        }
        
        public bool Handle(IGameContext gameContext, IEventEngine<IGameContext> eventEngine, Event @event)
        {
            // The console never handles non-keyboard events.
            if (!(@event is KeyboardEvent))
                return false;
            
            // If the console is open, consume all keyboard events.
            var keyPressEvent = @event as KeyPressEvent;
            if (this.m_Console.Open)
            {
                if (keyPressEvent != null &&
                    keyPressEvent.Key == Keys.OemTilde)
                    this.m_Console.Toggle();
                return true;
            }
        
            // We have a keyboard event, and the console isn't open.
            // If we aren't handling a key press, then let the event pass through.
            if (!(@event is KeyPressEvent))
                return false;
            
            // We have a key press; if it's the tilde then we open the console
            // and consume the event.
            if (keyPressEvent.Key == Keys.OemTilde)
            {
                this.m_Console.Toggle();
                return true;
            }
            
            // Let the event pass through.
            return false;
        }
    }
}

