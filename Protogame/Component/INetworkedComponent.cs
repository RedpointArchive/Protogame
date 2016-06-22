namespace Protogame
{
    public interface INetworkedComponent
    {
        bool ReceiveMessage(
            ComponentizedEntity entity,
            IGameContext gameContext, 
            IUpdateContext updateContext,
            MxDispatcher dispatcher,
            MxClient server,
            byte[] payload,
            uint protocolId);

        bool ReceiveMessage(
            ComponentizedEntity entity,
            IServerContext serverContext,
            IUpdateContext updateContext,
            MxDispatcher dispatcher,
            MxClient client,
            byte[] payload,
            uint protocolId);
    }
}
