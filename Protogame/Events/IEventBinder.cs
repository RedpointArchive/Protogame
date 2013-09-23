using Ninject.Syntax;

namespace Protogame
{
    public interface IEventBinder
    {
        int Priority { get; }
        void Assign(IResolutionRoot resolutionRoot);
        bool Handle(IGameContext gameContext, IEventEngine eventEngine, Event @event);
    }
}

