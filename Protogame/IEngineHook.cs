namespace Protogame
{
    public interface IEngineHook
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext);
        void Render(IGameContext gameContext, IRenderContext renderContext);
    }
}

