namespace Protogame
{
    public interface ISensor
    {
        void Render(IGameContext gameContext, IRenderContext renderContext);
        void Update(IGameContext gameContext, IUpdateContext updateContext);
    }
}