namespace Protogame
{
    public interface IEventEngine
    {
        void Fire(IGameContext gameContext, Event @event);
    }
}

