namespace Protogame
{
    public class CoroutineEngineHook : IEngineHook
    {
        private readonly ICoroutineScheduler _coroutineScheduler;

        private bool _hasEnabledGlobalCoroutines;

        public CoroutineEngineHook(ICoroutineScheduler coroutineScheduler)
        {
            _coroutineScheduler = coroutineScheduler;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _coroutineScheduler.Update(gameContext, updateContext);
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            _coroutineScheduler.Update(serverContext, updateContext);
        }
    }
}