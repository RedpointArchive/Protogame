// ReSharper disable CheckNamespace

using Protoinject;

namespace Protogame
{
    /// <summary>
    /// A general event binder for the collision system which dispatches events to
    /// only the entities involved in those events.  This event binder is automatically
    /// bound when you load <see cref="ProtogameCollisionModule"/>, but you can
    /// implement your own additional event binders if you want to dispatch physics
    /// events to other unrelated entities or services in your game.
    /// </summary>
    /// <module>Collision</module>
    public class GeneralPerPixelCollisionEventBinder : IEventBinder<IPerPixelCollisionEventContext>
    {
        /// <summary>
        /// The priority of this event binder.  Event binders with lower priority values
        /// will have the opportunity to consume events first.
        /// </summary>
        public int Priority => 100;

        /// <summary>
        /// Assigns the dependency injection kernel to this instance.
        /// </summary>
        /// <param name="kernel">The dependency injection kernel.</param>
        public void Assign(IKernel kernel)
        {
            // We don't require the dependency injection kernel in this implementation.
        }
        
        /// <summary>
        /// Handles physics events raised by the physics event engine.
        /// </summary>
        /// <param name="context">The physics event context, which doesn't contain any information.</param>
        /// <param name="eventEngine">The event engine for physics events.</param>
        /// <param name="event">The physics event that is being handled.</param>
        /// <returns>Whether the physics event was consumed by this event binder.</returns>
        public bool Handle(IPerPixelCollisionEventContext context, IEventEngine<IPerPixelCollisionEventContext> eventEngine, Event @event)
        {
            var perPixelEvent = @event as PerPixelCollisionEvent;

            if (perPixelEvent == null)
            {
                // Do not consume the event unless we're going to process it.
                return false;
            }

            var involvedEntities = new[]
            {
                perPixelEvent.Object1 as IEventListener<IPerPixelCollisionEventContext>,
                perPixelEvent.Object2 as IEventListener<IPerPixelCollisionEventContext>,
            };

            // Dispatch the event to the involved entities.
            involvedEntities[0]?.Handle(context, eventEngine, @event);
            involvedEntities[1]?.Handle(context, eventEngine, @event);
            return true;
        }
    }
}
