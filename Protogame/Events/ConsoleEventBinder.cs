namespace Protogame
{
    /// <summary>
    /// The console event binder.
    /// </summary>
    public class ConsoleEventBinder : StaticEventBinder<IGameContext>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        public override void Configure()
        {
            this.Bind<Event>(x => true).ToListener<ConsoleEventListener>();
        }
    }
}