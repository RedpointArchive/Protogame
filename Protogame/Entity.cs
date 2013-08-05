namespace Protogame
{
    public class Entity : IBoundingBox, IEntity
    {
        public virtual float X { get; set; }
        public virtual float Y { get; set; }
        public virtual float Z { get; set; }
        public virtual float Width { get; set; }
        public virtual float Height { get; set; }
        public virtual float Depth { get; set; }
        public virtual float XSpeed { get; set; }
        public virtual float YSpeed { get; set; }
        public virtual float ZSpeed { get; set; }

        public virtual void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.X += this.XSpeed;
            this.Y += this.YSpeed;
            this.Z += this.ZSpeed;
        }

        public virtual void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }
    }
}
