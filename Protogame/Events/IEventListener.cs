namespace Protogame
{
    /// <summary>
    /// The EventListener interface.
    /// </summary>
    /// <typeparam name="TContext">
    /// </typeparam>
    public interface IEventListener<TContext>
    {
        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="context">
        /// The context.
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
        bool Handle(TContext context, IEventEngine<TContext> eventEngine, Event @event);
    }
}