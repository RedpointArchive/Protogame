namespace Protogame
{
    public interface IEventListener<TContext>
    {
        bool Handle(TContext context, IEventEngine<TContext> eventEngine, Event @event);
    }
}

