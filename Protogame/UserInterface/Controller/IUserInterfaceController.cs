using System;

namespace Protogame
{
    public interface IUserInterfaceController : IDisposable
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext);

        IContainer LoadUserFragment(string name);

        void RegisterBehaviour<TContainerType>(string name, UserInterfaceBehaviourEvent @event, UserInterfaceBehaviourHandler<TContainerType> callback);

        bool Enabled { get; set; }
    }
}