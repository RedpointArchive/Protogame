namespace Protogame
{
    using System.Linq;

    /// <summary>
    /// Listens for incoming events and directs them to UI elements.
    /// </summary>
    public class UIEventListener : IEventListener<IGameContext>
    {
        /// <summary>
        /// The current skin being used by the UI system.
        /// </summary>
        private readonly ISkin m_Skin;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIEventListener"/> class.
        /// </summary>
        /// <param name="skin">
        /// The skin being used by the UI system.
        /// </param>
        public UIEventListener(ISkin skin)
        {
            this.m_Skin = skin;
        }

        /// <summary>
        /// Handles an incoming event and attempts to dispatch it to all known UI elements.
        /// </summary>
        /// <param name="context">
        /// The current game context.
        /// </param>
        /// <param name="eventEngine">
        /// The event engine that fired the event.
        /// </param>
        /// <param name="event">
        /// The event that was fired.
        /// </param>
        /// <returns>
        /// Whether the event was handled by this listener.
        /// </returns>
        public bool Handle(IGameContext context, IEventEngine<IGameContext> eventEngine, Event @event)
        {
            var worldHasCanvases = context.World as IHasCanvases;
            if (worldHasCanvases != null)
            {
                foreach (var kv in worldHasCanvases.Canvases)
                {
                    if (kv.Key.HandleEvent(this.m_Skin, kv.Value, context, @event))
                    {
                        return true;
                    }
                }
            }

            foreach (var kv in context.World.Entities.OfType<IHasCanvases>().SelectMany(x => x.Canvases))
            {
                if (kv.Key.HandleEvent(this.m_Skin, kv.Value, context, @event))
                {
                    return true;
                }
            }

            return false;
        }
    }
}