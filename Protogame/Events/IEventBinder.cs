namespace Protogame
{
    public interface IEventBinder
    {
        int Priority { get; }
        bool Handle(IGameContext gameContext, IEventEngine eventEngine, Event @event);
    }
}

