namespace Protogame
{
    public class RenderAutoCacheEngineHook : IEngineHook
    {
        private readonly IRenderAutoCache _renderAutoCache;

        public RenderAutoCacheEngineHook(IRenderAutoCache renderAutoCache)
        {
            _renderAutoCache = renderAutoCache;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _renderAutoCache.Update(gameContext, updateContext);
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
        }
    }
}