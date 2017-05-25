// ReSharper disable CheckNamespace

using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// The default implementation of a game entity that doesn't use components.  You can
    /// inherit from this class to make implementing <see cref="IEntity"/> easier.
    /// </summary>
    /// <module>Core API</module>
    public class Entity : IBoundingBox, IEntity
    {
        private readonly IFinalTransform _finalTransform;

        /// <summary>
        /// Initializes a new entity.
        /// </summary>
        public Entity()
        {
            _finalTransform = new DefaultFinalTransform(this, null);
            Transform = new DefaultTransform();
        }
        
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
        /// Gets or sets the x speed.
        /// </summary>
        /// <value>
        /// The x speed.
        /// </value>
        public virtual float XSpeed { get; set; }

        /// <summary>
        /// Gets or sets the y speed.
        /// </summary>
        /// <value>
        /// The y speed.
        /// </value>
        public virtual float YSpeed { get; set; }

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
            Transform.LocalPosition += new Vector3(XSpeed, YSpeed, ZSpeed);
        }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform => _finalTransform;
    }
}