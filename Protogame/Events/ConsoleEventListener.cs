namespace Protogame
{
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The console event listener.
    /// </summary>
    public class ConsoleEventListener : IEventListener<IGameContext>
    {
        /// <summary>
        /// The m_ console.
        /// </summary>
        private readonly IConsole m_Console;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleEventListener"/> class.
        /// </summary>
        /// <param name="console">
        /// The console.
        /// </param>
        public ConsoleEventListener(IConsole console)
        {
            this.m_Console = console;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="eventEngine">
        /// The event engine.
        /// </param>
        /// <param name="event">
        /// The event.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Handle(IGameContext gameContext, IEventEngine<IGameContext> eventEngine, Event @event)
        {
            // The console never handles non-keyboard events.
            if (!(@event is KeyboardEvent))
            {
                return false;
            }

            // If the console is open, consume all keyboard events.
            var keyPressEvent = @event as KeyPressEvent;
            if (this.m_Console.Open)
            {
                if (keyPressEvent != null && keyPressEvent.Key == Keys.OemTilde)
                {
                    this.m_Console.Toggle();
                }

                return true;
            }

            // We have a keyboard event, and the console isn't open.
            // If we aren't handling a key press, then let the event pass through.
            if (!(@event is KeyPressEvent))
            {
                return false;
            }

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