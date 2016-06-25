namespace Protogame
{
    public interface INetworkIdentifiable
    {
        void ReceiveNetworkIDFromServer(IGameContext gameContext, IUpdateContext updateContext, int identifier, int initialFrameTick);

        void ReceivePredictedNetworkIDFromClient(IServerContext serverContext, IUpdateContext updateContext, MxClient client, int predictedIdentifier);
    }
}
