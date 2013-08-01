namespace Protogame
{
    public interface IEntity : IHasPosition
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext);
        void Render(IGameContext gameContext, IRenderContext renderContext);
    }
}
