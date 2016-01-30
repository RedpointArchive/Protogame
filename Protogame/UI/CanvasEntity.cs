namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The canvas entity.
    /// </summary>
    public class CanvasEntity : Entity, IHasCanvases
    {
        /// <summary>
        /// The m_ skin.
        /// </summary>
        private readonly ISkin m_Skin;

        private Rectangle m_LastRenderBounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasEntity"/> class.
        /// </summary>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public CanvasEntity(ISkin skin)
        {
            if (skin == null)
            {
                throw new ArgumentNullException("skin");
            }

            this.m_Skin = skin;
            this.Windows = new List<Window>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasEntity"/> class.
        /// </summary>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="canvas">
        /// The canvas.
        /// </param>
        public CanvasEntity(ISkin skin, Canvas canvas)
            : this(skin)
        {
            this.Canvas = canvas;
        }

        /// <summary>
        /// Gets or sets the canvas.
        /// </summary>
        /// <value>
        /// The canvas.
        /// </value>
        public Canvas Canvas { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        public List<Window> Windows { get; set; }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                return;
            }

            var bounds = gameContext.Window.ClientBounds;
            bounds.X = 0;
            bounds.Y = 0;

            this.m_LastRenderBounds = bounds;

            base.Render(gameContext, renderContext);

            if (this.Canvas != null)
            {
                this.Canvas.Draw(renderContext, this.m_Skin, bounds);
            }

            foreach (var window in this.Windows.OrderBy(x => x.Order))
            {
                window.Draw(renderContext, this.m_Skin, window.Bounds);
            }
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
        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.Width = gameContext.Window.ClientBounds.Width;
            this.Height = gameContext.Window.ClientBounds.Height;

            base.Update(gameContext, updateContext);

            var stealFocus = false;
            foreach (var window in this.Windows.OrderBy(x => x.Order))
            {
                window.Update(this.m_Skin, window.Bounds, gameContext.GameTime, ref stealFocus);
                if (stealFocus)
                {
                    return;
                }
            }

            if (this.Canvas != null)
            {
                var bounds = gameContext.Window.ClientBounds;
                bounds.X = 0;
                bounds.Y = 0;

                this.Canvas.Update(this.m_Skin, bounds, gameContext.GameTime, ref stealFocus);
            }

            if (stealFocus)
            {
                return;
            }
        }

        /// <summary>
        /// Gets the Canvas objects.
        /// </summary>
        /// <value>
        /// The Canvas objects.
        /// </value>
        public IEnumerable<KeyValuePair<Canvas, Rectangle>> Canvases
        {
            get
            {
                return new[] { new KeyValuePair<Canvas, Rectangle>(this.Canvas, this.m_LastRenderBounds) };
            }
        }
    }
}