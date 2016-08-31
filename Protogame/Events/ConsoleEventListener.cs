namespace Protogame
{
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Handles incoming events and passes them to the in-game console when appropriate.
    /// </summary>
    /// <module>Events</module>
    /// <internal>True</internal>
    public class ConsoleEventListener : IEventListener<IGameContext>
    {
        /// <summary>
        /// The in-game console.
        /// </summary>
        private readonly IConsole _console;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleEventListener"/> class.
        /// </summary>
        /// <param name="console">
        /// The in-game console.
        /// </param>
        public ConsoleEventListener(IConsole console)
        {
            _console = console;
        }

        /// <summary>
        /// Handles the event as appropriate for the in-game console.
        /// </summary>
        /// <param name="gameContext">
        /// The current game context.
        /// </param>
        /// <param name="eventEngine">
        /// The current event engine.
        /// </param>
        /// <param name="event">
        /// The event that is being handled.
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
            if (_console.State == ConsoleState.Open || _console.State == ConsoleState.FullOpen)
            {
                if (keyPressEvent != null && keyPressEvent.Key == Keys.OemTilde)
                {
                    _console.Toggle();
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
                _console.Toggle();
                return true;
            }

            // Let the event pass through.
            return false;
        }
    }
}