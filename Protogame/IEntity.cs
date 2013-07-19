namespace Protogame
{
    public interface IEntity
    {
        float X { get; set; }
        float Y { get; set; }
        
        void Update(IGameContext gameContext, IUpdateContext updateContext);
        void Render(IGameContext gameContext, IRenderContext renderContext);
    }
}
