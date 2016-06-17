namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The fixed container.
    /// </summary>
    public class FixedContainer : BaseContainer, IContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedContainer"/> class.
        /// </summary>
        /// <param name="absolute">
        /// The absolute.
        /// </param>
        public FixedContainer(Rectangle absolute)
        {
            this.AbsoluteRectangle = absolute;
        }

        /// <summary>
        /// Gets or sets the absolute rectangle.
        /// </summary>
        /// <value>
        /// The absolute rectangle.
        /// </value>
        public Rectangle AbsoluteRectangle { get; set; }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawFixedContainer(context, layout, this);
            if (this.Child != null)
            {
                this.Child.Draw(context, skin, this.AbsoluteRectangle);
            }
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        /// <param name="stealFocus">
        /// The steal focus.
        /// </param>
        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            if (this.Child != null)
            {
                this.Child.Update(skin, this.AbsoluteRectangle, gameTime, ref stealFocus);
            }
        }

        /// <summary>
        /// Requests that the UI container handle the specified event or return false.
        /// </summary>
        /// <param name="skin">
        /// The UI skin.
        /// </param>
        /// <param name="layout">
        /// The layout for the element.
        /// </param>
        /// <param name="context">
        /// The current game context.
        /// </param>
        /// <param name="event">
        /// The event that was raised.
        /// </param>
        /// <returns>
        /// Whether or not this UI element handled the event.
        /// </returns>
        public bool HandleEvent(ISkin skin, Rectangle layout, IGameContext context, Event @event)
        {
            if (this.Child != null)
            {
                return this.Child.HandleEvent(skin, this.AbsoluteRectangle, context, @event);
            }

            return false;
        }
    }
}