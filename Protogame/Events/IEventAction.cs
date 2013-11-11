namespace Protogame
{
    public interface IEventAction<TContext>
    {
        void Handle(TContext context, Event @event);
    }
}

