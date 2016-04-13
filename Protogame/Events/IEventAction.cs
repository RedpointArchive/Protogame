namespace Protogame
{
    /// <summary>
    /// Represents an action to be taken when the filter in the event binder matches an event.
    /// <para>
    /// You should implement this class to respond to events.
    /// </para>
    /// </summary>
    /// <typeparam name="TContext">
    /// The context of the event.
    /// </typeparam>
    /// <module>Events</module>
    public interface IEventAction<TContext>
    {
        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="context">
        /// The event context.
        /// </param>
        /// <param name="event">
        /// The event that is being handled.
        /// </param>
        void Handle(TContext context, Event @event);
    }
}