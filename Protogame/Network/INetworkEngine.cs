using System;
using System.Collections.Generic;
using System.Net;

namespace Protogame
{
    public interface INetworkEngine
    {
        void AttachDispatcher(IWorld world, MxDispatcher dispatcher);

        void AttachDispatcher(IServerWorld world, MxDispatcher dispatcher);

        void Update(IGameContext gameContext, IUpdateContext updateContext);

        void Update(IServerContext serverContext, IUpdateContext updateContext);

        MxDispatcher[] CurrentDispatchers { get; }

        IEnumerable<KeyValuePair<int, object>> ListObjectsByNetworkId();

        T FindObjectByNetworkId<T>(int id);

        object FindObjectByNetworkId(int id);

        void RegisterObjectAsNetworkId(int id, object obj);

        void DeregisterObjectFromNetworkId(int id);

        int ClientRenderDelayTicks { get; set; }

        void Send<T>(MxDispatcher dispatcher, IPEndPoint target, T message, bool reliable = false);

        Dictionary<Type, int> GetSizeOfMessagesSentLastFrame();

        Dictionary<Type, int> GetCountOfMessagesSentLastFrame();

        Dictionary<Type, int> GetSizeOfMessagesReceivedLastFrame();

        Dictionary<Type, int> GetCountOfMessagesReceivedLastFrame();
    }
}
