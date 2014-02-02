namespace Protogame
{
    /// <summary>
    /// The entity.
    /// </summary>
    public class Entity : IBoundingBox, IEntity
    {
        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public virtual float Depth { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public virtual float Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public virtual float Width { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public virtual float X { get; set; }

        /// <summary>
        /// Gets or sets the x speed.
        /// </summary>
        /// <value>
        /// The x speed.
        /// </value>
        public virtual float XSpeed { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public virtual float Y { get; set; }

        /// <summary>
        /// Gets or sets the y speed.
        /// </summary>
        /// <value>
        /// The y speed.
        /// </value>
        public virtual float YSpeed { get; set; }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        /// <value>
        /// The z.
        /// </value>
        public virtual float Z { get; set; }

        /// <summary>
        /// Gets or sets the z speed.
        /// </summary>
        /// <value>
        /// The z speed.
        /// </value>
        public virtual float ZSpeed { get; set; }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        public virtual void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="updateContext">
        /// The update context.
        /// </param>
        public virtual void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.X += this.XSpeed;
            this.Y += this.YSpeed;
            this.Z += this.ZSpeed;
        }
    }
}