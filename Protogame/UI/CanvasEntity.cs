using System;
using Protogame;
using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    public class CanvasEntity : Entity
    {
        private ISkin m_Skin;
        public Canvas Canvas { get; set; }
        public List<Window> Windows { get; set; }

        public CanvasEntity(ISkin skin)
        {
            if (skin == null)
                throw new ArgumentNullException("skin");
            this.m_Skin = skin;
            this.Windows = new List<Window>();
        }

        public CanvasEntity(ISkin skin, Canvas canvas)
            : this(skin)
        {
            this.Canvas = canvas;
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.Width = gameContext.Window.ClientBounds.Width;
            this.Height = gameContext.Window.ClientBounds.Height;
        
            base.Update(gameContext, updateContext);
            
            var stealFocus = false;
            foreach (var window in this.Windows.OrderBy(x => x.Order))
            {
                window.Update(
                    this.m_Skin,
                    window.Bounds,
                    gameContext.GameTime,
                    ref stealFocus);
                if (stealFocus)
                    return;
            }

            if (this.Canvas != null)
            {
                this.Canvas.Update(
                    this.m_Skin,
                    gameContext.Window.ClientBounds,
                    gameContext.GameTime,
                    ref stealFocus);
            }
            if (stealFocus)
                return;
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                return;
            }

            base.Render(gameContext, renderContext);

            if (this.Canvas != null)
                this.Canvas.Draw(renderContext, this.m_Skin, gameContext.Window.ClientBounds);
            
            foreach (var window in this.Windows.OrderBy(x => x.Order))
                window.Draw(renderContext, this.m_Skin, window.Bounds);
        }
    }
}

