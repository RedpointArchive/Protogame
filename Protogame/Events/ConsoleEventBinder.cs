namespace Protogame
{
    /// <summary>
    /// Binds events such that the in-game console will receive input.
    /// <remarks>
    /// You shouldn't need to set up this event binder manually, as it is handled when
    /// the <see cref="ProtogameEventsIoCModule"/> is loaded.
    /// </remarks>
    /// </summary>
    /// <module>Events</module>
    /// <internal>True</internal>
    public class ConsoleEventBinder : StaticEventBinder<IGameContext>
    {
        /// <summary>
        /// Configures the event system so that events are propagated to the console event listener.
        /// </summary>
        public override void Configure()
        {
            this.Bind<Event>(x => true).ToListener<ConsoleEventListener>();
        }
    }
}