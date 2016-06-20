namespace Protogame
{
    public interface INetworkedComponent
    {
        bool ReceiveMessage(
            IGameContext gameContext, 
            IUpdateContext updateContext,
            MxDispatcher dispatcher,
            MxClient server,
            byte[] payload,
            uint protocolId);

        bool ReceiveMessage(
            IServerContext serverContext,
            IUpdateContext updateContext,
            MxDispatcher dispatcher,
            MxClient client,
            byte[] payload,
            uint protocolId);
    }
}
