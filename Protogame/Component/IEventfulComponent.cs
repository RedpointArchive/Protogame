namespace Protogame
{
    /// <summary>
    /// A component which handles events (such as input events).  A component
    /// which implements will have <see cref="Handle"/> called when an event
    /// is propagated to the entity that contains it.  Returning <c>true</c>
    /// from <see cref="Handle"/> will consume the event, and prevent it from
    /// being handled by any components further in the tree.
    /// </summary>
    /// <module>Component</module>
    public interface IEventfulComponent
    {
        /// <summary>
        /// Called by the entity or parent component when the component is
        /// requested to handle an event.
        /// </summary>
        /// <param name="componentizedEntity">The entity containing all components.</param>
        /// <param name="gameContext">The game context.</param>
        /// <param name="eventEngine">The event engine from which the event was fired.</param>
        /// <param name="event">The event to be handled.</param>
        /// <returns><c>true</c> if the event should be consumed, <c>false</c> otherwise.</returns>
        bool Handle(ComponentizedEntity componentizedEntity, IGameContext gameContext, IEventEngine<IGameContext> eventEngine, Event @event);
    }
}
