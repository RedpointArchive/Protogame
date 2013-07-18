namespace Protogame
{
    public interface IEntity
    {
        float X { get; set; }
        float Y { get; set; }
        
        void Update(IUpdateContext context);
        void Render(IRenderContext context);
    }
}
