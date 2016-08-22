using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Protogame
{    
    public class CanvasEntity : Entity, IHasCanvases
    {
        private readonly ISkinLayout _skinLayout;
        private readonly ISkinDelegator _skinDelegator;

        private Rectangle _lastRenderBounds;
        
        public CanvasEntity(ISkinLayout skinLayout, ISkinDelegator skinDelegator)
        {
            _skinLayout = skinLayout;
            _skinDelegator = skinDelegator;
            Windows = new List<Window>();
        }
        
        public Canvas Canvas { get; set; }
        
        public List<Window> Windows { get; set; }
        
        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.IsCurrentRenderPass<ICanvasRenderPass>())
            {
                var bounds = gameContext.Window.ClientBounds;
                bounds.X = 0;
                bounds.Y = 0;

                _lastRenderBounds = bounds;

                base.Render(gameContext, renderContext);

                Canvas?.Render(renderContext, _skinLayout, _skinDelegator, bounds);

                foreach (var window in Windows.OrderBy(x => x.Order))
                {
                    window.Render(renderContext, _skinLayout, _skinDelegator, window.Bounds);
                }
            }
        }
        
        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            Width = gameContext.Window.ClientBounds.Width;
            Height = gameContext.Window.ClientBounds.Height;

            base.Update(gameContext, updateContext);

            var stealFocus = false;
            foreach (var window in Windows.OrderBy(x => x.Order))
            {
                window.Update(_skinLayout, window.Bounds, gameContext.GameTime, ref stealFocus);
                if (stealFocus)
                {
                    return;
                }
            }

            if (Canvas != null)
            {
                var bounds = gameContext.Window.ClientBounds;
                bounds.X = 0;
                bounds.Y = 0;

                Canvas.Update(_skinLayout, bounds, gameContext.GameTime, ref stealFocus);
            }

            if (stealFocus)
            {
                return;
            }
        }
        
        public IEnumerable<KeyValuePair<Canvas, Rectangle>> Canvases
        {
            get
            {
                return new[] { new KeyValuePair<Canvas, Rectangle>(Canvas, _lastRenderBounds) };
            }
        }
    }
}