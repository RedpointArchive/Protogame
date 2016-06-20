namespace Protogame
{
    public class NetworkEngineHook : IEngineHook
    {
        private readonly INetworkEngine _networkEngine;

        public NetworkEngineHook(INetworkEngine networkEngine)
        {
            _networkEngine = networkEngine;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _networkEngine.Update(gameContext, updateContext);
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            _networkEngine.Update(serverContext, updateContext);
        }
    }
}
