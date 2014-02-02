namespace Protogame
{
    using System.Linq;
    using Ninject.Syntax;

    /// <summary>
    /// The default event engine.
    /// </summary>
    /// <typeparam name="TContext">
    /// </typeparam>
    public class DefaultEventEngine<TContext> : IEventEngine<TContext>
    {
        /// <summary>
        /// The m_ event binders.
        /// </summary>
        private readonly IEventBinder<TContext>[] m_EventBinders;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEventEngine{TContext}"/> class.
        /// </summary>
        /// <param name="resolutionRoot">
        /// The resolution root.
        /// </param>
        /// <param name="eventBinders">
        /// The event binders.
        /// </param>
        public DefaultEventEngine(IResolutionRoot resolutionRoot, IEventBinder<TContext>[] eventBinders)
        {
            this.m_EventBinders = eventBinders;
            foreach (var eventBinder in this.m_EventBinders)
            {
                eventBinder.Assign(resolutionRoot);
            }
        }

        /// <summary>
        /// The fire.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="event">
        /// The event.
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