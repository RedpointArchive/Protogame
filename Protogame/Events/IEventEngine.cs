namespace Protogame
{
    public interface IEventEngine<TContext>
    {
        void Fire(TContext context, Event @event);
    }
}

