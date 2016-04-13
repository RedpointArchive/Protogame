using Protoinject;

namespace Protogame
{
    /// <summary>
    /// An event binder, which accepts events raised by the <see cref="IEventEngine{TContext}"/> and propagates them to actions and listeners.
    /// <para>
    /// You probably want to derive <see cref="StaticEventBinder{TContext}"/>, which provides an easy way for filtering events and assigning them
    /// to actions and listeners.  This interface is an advanced mechanism, allowing you as a developer to have an event binder that allows remapping and
    /// addition of event bindings at runtime.
    /// </para>
    /// </summary>
    /// <typeparam name="TContext">
    /// The context for events.
    /// </typeparam>
    /// <module>Events</module>
    public interface IEventBinder<TContext>
    {
        /// <summary>
        /// Gets the priority of this event binder.
        /// <para>
        /// When event binders are registered in the dependency injection kernel, events are passed to
        /// them in the order of their priority.
        /// </para>
        /// </summary>
        /// <value>
        /// The priority of this event binder.
        /// </value>
        int Priority { get; }

        /// <summary>
        /// Assigns the dependency injection kernel to this event binder.
        /// </summary>
        /// <param name="kernel">
        /// The dependency injection kernel.
        /// </param>
        void Assign(IKernel kernel);

        /// <summary>
        /// Requests to handle the specified event.  Returns <c>true</c> if the event was consumed
        /// by this event binder, <c>false</c> otherwise.
        /// </summary>
        /// <param name="context">
        /// The context for events.
        /// </param>
        /// <param name="eventEngine">
        /// The event engine.
        /// </param>
        /// <param name="event">
        /// The event that is being processed.
        /// </param>
        /// <returns>
        /// Whether or not this event binder consumes the event.  If the event binder consumes
        /// the event, it is not passed onto any other event binders that are registered.
        /// </returns>
        bool Handle(TContext context, IEventEngine<TContext> eventEngine, Event @event);
    }
}