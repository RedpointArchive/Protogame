namespace Protogame
{
    /// <summary>
    /// The event binder that connects events up to UI elements.
    /// </summary>
    public class UIEventBinder : StaticEventBinder<IGameContext>
    {
        /// <summary>
        /// Configures the event bindings.
        /// </summary>
        public override void Configure()
        {
            this.Bind<Event>(x => true).ToListener<UIEventListener>();
        }
    }
}