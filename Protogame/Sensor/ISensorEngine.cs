namespace Protogame
{
    public interface ISensorEngine
    {
        void Render(IGameContext gameContext, IRenderContext renderContext);
        void Update(IGameContext gameContext, IUpdateContext updateContext);
        void Register(ISensor sensor);
        void Deregister(ISensor sensor);
    }
}