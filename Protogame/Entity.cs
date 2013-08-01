namespace Protogame
{
    public class Entity : IBoundingBox, IEntity
    {
        public virtual float X { get; set; }
        public virtual float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float XSpeed { get; set; }
        public float YSpeed { get; set; }

        public virtual void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.X += this.XSpeed;
            this.Y += this.YSpeed;
        }

        public virtual void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }
    }
}
