using System;
using System.Collections;
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

        void Send<T>(MxDispatcher dispatcher, MxClientGroup target, T message, bool reliable = false);

        IEnumerable<INetworkFrame> GetRecentFrames();

        void LogSynchronisationEvent(string log);
    }
}
