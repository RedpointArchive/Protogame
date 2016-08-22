using System;
using HiveMP.Lobby.Model;

namespace Protogame
{
    public interface IUserInterfaceController : IDisposable
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext);

        IContainer LoadUserFragment(string name);

        void RegisterBehaviour<TContainerType>(string name, UserInterfaceBehaviourEvent @event, Action<TContainerType, IUserInterfaceController, IGameContext, IUpdateContext> callback);
    }
}