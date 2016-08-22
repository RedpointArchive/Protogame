using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicCanvasSkinRenderer : ISkinRenderer<Canvas>
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IBasicSkin _basicSkin;

        public BasicCanvasSkinRenderer(I2DRenderUtilities renderUtilities, IBasicSkin basicSkin)
        {
            _renderUtilities = renderUtilities;
            _basicSkin = basicSkin;
        }

        public void Render(IRenderContext renderContext, Rectangle layout, Canvas canvas)
        {
            _renderUtilities.RenderRectangle(renderContext, layout, _basicSkin.BackSurfaceColor, true);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, Canvas container)
        {
            throw new NotSupportedException();
        }
    }
}
