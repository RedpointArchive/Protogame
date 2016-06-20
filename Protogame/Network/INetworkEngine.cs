namespace Protogame
{
    public interface INetworkEngine
    {
        void AttachDispatcher(IWorld world, MxDispatcher dispatcher);

        void AttachDispatcher(IServerWorld world, MxDispatcher dispatcher);

        void Update(IGameContext gameContext, IUpdateContext updateContext);

        void Update(IServerContext serverContext, IUpdateContext updateContext);

        MxDispatcher[] CurrentDispatchers { get; }
    }
}
