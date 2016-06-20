namespace Protogame
{
    public interface INetworkedServerWorld : IServerWorld
    {
        bool ReceiveMessage(
            IServerContext serverContext,
            IUpdateContext updateContext,
            MxDispatcher dispatcher,
            MxClient client,
            byte[] payload,
            uint protocolId);

        bool ClientConnected(
            IServerContext serverContext, 
            IUpdateContext updateContext,
            MxDispatcher dispatcher, 
            MxClient client);

        bool ClientDisconnected(
            IServerContext serverContext, 
            IUpdateContext updateContext, 
            MxDispatcher dispatcher, 
            MxClient client);
    }
}
