namespace Protogame
{
    public interface IEventListener
    {
        bool Handle(IGameContext gameContext, IEventEngine eventEngine, Event @event);
    }
}

