using Ninject.Syntax;

namespace Protogame
{
    public interface IEventBinder<TContext>
    {
        int Priority { get; }
        void Assign(IResolutionRoot resolutionRoot);
        bool Handle(TContext context, IEventEngine<TContext> eventEngine, Event @event);
    }
}

