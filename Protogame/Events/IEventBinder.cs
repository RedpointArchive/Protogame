namespace Protogame
{
    using Ninject.Syntax;

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
        /// <param name="resolutionRoot">
        /// The resolution root.
        /// </param>
        void Assign(IResolutionRoot resolutionRoot);

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