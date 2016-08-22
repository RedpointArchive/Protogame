using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicFontViewerSkinRenderer : ISkinRenderer<FontViewer>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;
        private readonly I2DRenderUtilities _renderUtilities;

        public BasicFontViewerSkinRenderer(IBasicSkinHelper basicSkinHelper, I2DRenderUtilities renderUtilities, IAssetManagerProvider assetManagerProvider)
        {
            _basicSkinHelper = basicSkinHelper;
            _renderUtilities = renderUtilities;
        }

        public void Render(IRenderContext renderContext, Rectangle layout, FontViewer fontViewer)
        {
            _basicSkinHelper.DrawSunken(renderContext, layout);

            if (fontViewer.Font != null && fontViewer.Font.PlatformData != null)
            {
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(layout.X, layout.Y),
                    "Font Example",
                    fontViewer.Font);
            }
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, FontViewer container)
        {
            throw new NotSupportedException();
        }
    }
}
