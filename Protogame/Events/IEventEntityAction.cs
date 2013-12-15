namespace Protogame
{
    public interface IEventEntityAction<TEntity> where TEntity : IEntity
    {
        void Handle(IGameContext context, TEntity entity, Event @event);
    }
}

