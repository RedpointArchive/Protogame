namespace Protogame
{
    public interface IEventEntityAction<TEntity> where TEntity : IEntity
    {
        void Handle(TEntity entity, Event @event);
    }
}

