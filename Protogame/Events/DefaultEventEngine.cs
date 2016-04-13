using Protoinject;

namespace Protogame
{
    using System.Linq;

    /// <summary>
    /// The default implementation for an <see cref="IEventEngine{TContext}"/>.
    /// </summary>
    /// <typeparam name="TContext">
    /// The context that is being passed to events.
    /// </typeparam>
    /// <module>Events</module>
    /// <internal>True</internal>
    public class DefaultEventEngine<TContext> : IEventEngine<TContext>
    {
        /// <summary>
        /// The registered event binders.
        /// </summary>
        private readonly IEventBinder<TContext>[] m_EventBinders;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEventEngine{TContext}"/> class.
        /// </summary>
        /// <param name="kernel">
        /// The dependency injection kernel.
        /// </param>
        /// <param name="eventBinders">
        /// The registered event binders.
        /// </param>
        public DefaultEventEngine(IKernel kernel, IEventBinder<TContext>[] eventBinders)
        {
            this.m_EventBinders = eventBinders;
            foreach (var eventBinder in this.m_EventBinders)
            {
                eventBinder.Assign(kernel);
            }
        }

        /// <summary>
        /// Called by code that wants to fire an event in the event system.
        /// </summary>
        /// <param name="context">
        /// The event context.
        /// </param>
        /// <param name="event">
        /// The event to fire.
        /// </param>
        public void Fire(TContext context, Event @event)
        {
            foreach (var eventBinder in this.m_EventBinders.OrderByDescending(x => x.Priority))
            {
                if (eventBinder.Handle(context, this, @event))
                {
                    break;
                }
            }
        }
    }
}