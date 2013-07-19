using System;
using Protogame;

namespace Protogame
{
    public class CanvasEntity : Entity
    {
        private ISkin m_Skin;
        public Canvas Canvas { get; set; }

        public CanvasEntity(ISkin skin)
        {
            if (skin == null)
                throw new ArgumentNullException("skin");
            this.m_Skin = skin;
        }

        public CanvasEntity(ISkin skin, Canvas canvas)
            : this(skin)
        {
            this.Canvas = canvas;
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            base.Update(gameContext, updateContext);

            if (this.Canvas != null)
            {
                var stealFocus = false;
                this.Canvas.Update(
                    this.m_Skin,
                    gameContext.Window.ClientBounds,
                    gameContext.GameTime,
                    ref stealFocus);
            }
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            base.Render(gameContext, renderContext);

            if (this.Canvas != null)
                this.Canvas.Draw(renderContext, this.m_Skin, gameContext.Window.ClientBounds);
        }
    }
}

