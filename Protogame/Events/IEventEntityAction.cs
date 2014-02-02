namespace Protogame
{
    /// <summary>
    /// The EventEntityAction interface.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    public interface IEventEntityAction<TEntity>
        where TEntity : IEntity
    {
        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="event">
        /// The event.
        /// </param>
        void Handle(IGameContext context, TEntity entity, Event @event);
    }
}