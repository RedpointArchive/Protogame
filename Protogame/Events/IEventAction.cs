namespace Protogame
{
    /// <summary>
    /// The EventAction interface.
    /// </summary>
    /// <typeparam name="TContext">
    /// </typeparam>
    public interface IEventAction<TContext>
    {
        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="event">
        /// The event.
        /// </param>
        void Handle(TContext context, Event @event);
    }
}