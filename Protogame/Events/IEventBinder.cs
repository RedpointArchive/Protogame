using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The EventBinder interface.
    /// </summary>
    /// <typeparam name="TContext">
    /// </typeparam>
    public interface IEventBinder<TContext>
    {
        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        int Priority { get; }

        /// <summary>
        /// The assign.
        /// </summary>
        /// <param name="kernel">
        /// The dependency injection kernel.
        /// </param>
        void Assign(IKernel kernel);

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