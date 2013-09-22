namespace Protogame
{
    public interface IEventAction
    {
        void Handle(IGameContext gameContext, Event @event);
    }
}

