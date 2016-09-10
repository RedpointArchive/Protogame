using System.Linq;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// Listens for incoming events and directs them to UI elements.
    /// </summary>
    /// <module>User Interface</module>
    public class UIEventListener : IEventListener<IGameContext>
    {
        private readonly ISkinLayout _skinLayout;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIEventListener"/> class.
        /// </summary>
        /// <param name="skin">
        /// The skin being used by the UI system.
        /// </param>
        public UIEventListener([Optional] ISkinLayout skinLayout, IKernel kernel)
        {
            _skinLayout = skinLayout ?? new BasicSkinLayout();
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
            if (worldHasCanvases != null && worldHasCanvases.CanvasesEnabled)
            {
                foreach (var kv in worldHasCanvases.Canvases)
                {
                    if (kv.Key.HandleEvent(_skinLayout, kv.Value, context, @event))
                    {
                        return true;
                    }
                }
            }

            foreach (var kv in context.World.GetEntitiesForWorld(context.Hierarchy).OfType<IHasCanvases>().Where(x => x.CanvasesEnabled).SelectMany(x => x.Canvases))
            {
                if (kv.Key.HandleEvent(_skinLayout, kv.Value, context, @event))
                {
                    return true;
                }
            }

            return false;
        }
    }
}