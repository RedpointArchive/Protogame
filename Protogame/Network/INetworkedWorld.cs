namespace Protogame
{
    public interface INetworkedWorld : IWorld
    {
        bool ReceiveMessage(
            IGameContext gameContext,
            IUpdateContext updateContext,
            MxDispatcher dispatcher,
            MxClient server,
            byte[] payload,
            uint protocolId);

        bool ClientConnected(
            IGameContext gameContext,
            IUpdateContext updateContext,
            MxDispatcher dispatcher,
            MxClient server);

        bool ClientDisconnected(
            IGameContext gameContext,
            IUpdateContext updateContext, 
            MxDispatcher dispatcher,
            MxClient server);
    }
}
