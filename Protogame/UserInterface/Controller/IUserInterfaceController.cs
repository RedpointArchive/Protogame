using System;

namespace Protogame
{
    public interface IUserInterfaceController : IDisposable
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext);

        void RegisterBehaviour<TContainerType>(string name, UserInterfaceBehaviourEvent @event, Action<TContainerType, IGameContext, IUpdateContext> callback);
    }
}