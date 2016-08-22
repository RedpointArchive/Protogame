using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicSkinHelper : IBasicSkinHelper
    {
        private readonly IBasicSkin _basicSkin;
        private readonly I2DRenderUtilities _renderUtilities;

        public BasicSkinHelper(IBasicSkin basicSkin, I2DRenderUtilities renderUtilities)
        {
            _basicSkin = basicSkin;
            _renderUtilities = renderUtilities;
        }

        public void DrawFlat(IRenderContext context, Rectangle layout)
        {
            _renderUtilities.RenderRectangle(context, layout, _basicSkin.SurfaceColor, true);
            _renderUtilities.RenderRectangle(context, layout, _basicSkin.LightEdgeColor);
        }

        public void DrawRaised(IRenderContext context, Rectangle layout)
        {
            _renderUtilities.RenderRectangle(context, layout, _basicSkin.SurfaceColor, true);
            _renderUtilities.RenderLine(
                context,
                new Vector2(layout.X, layout.Y + layout.Height - 1),
                new Vector2(layout.X + layout.Width, layout.Y + layout.Height - 1),
                _basicSkin.DarkEdgeColor);
            _renderUtilities.RenderLine(
                context,
                new Vector2(layout.X + layout.Width - 1, layout.Y),
                new Vector2(layout.X + layout.Width - 1, layout.Y + layout.Height),
                _basicSkin.DarkEdgeColor);
            _renderUtilities.RenderLine(
                context,
                new Vector2(layout.X, layout.Y),
                new Vector2(layout.X + layout.Width, layout.Y),
                _basicSkin.LightEdgeColor);
            _renderUtilities.RenderLine(
                context,
                new Vector2(layout.X, layout.Y),
                new Vector2(layout.X, layout.Y + layout.Height),
                _basicSkin.LightEdgeColor);
        }

        public void DrawSunken(IRenderContext context, Rectangle layout)
        {
            _renderUtilities.RenderRectangle(context, layout, _basicSkin.DarkSurfaceColor);
            _renderUtilities.RenderLine(
                context,
                new Vector2(layout.X, layout.Y + layout.Height - 1),
                new Vector2(layout.X + layout.Width, layout.Y + layout.Height - 1),
                _basicSkin.LightEdgeColor);
            _renderUtilities.RenderLine(
                context,
                new Vector2(layout.X + layout.Width - 1, layout.Y),
                new Vector2(layout.X + layout.Width - 1, layout.Y + layout.Height),
                _basicSkin.LightEdgeColor);
            _renderUtilities.RenderLine(
                context,
                new Vector2(layout.X, layout.Y),
                new Vector2(layout.X + layout.Width, layout.Y),
                _basicSkin.DarkEdgeColor);
            _renderUtilities.RenderLine(
                context,
                new Vector2(layout.X, layout.Y),
                new Vector2(layout.X, layout.Y + layout.Height),
                _basicSkin.DarkEdgeColor);
        }
    }
}