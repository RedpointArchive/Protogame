namespace Protogame
{
    public interface IRenderableComponent
    {
        void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext);
    }
}