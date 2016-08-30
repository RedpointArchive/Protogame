using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicTextureViewerSkinRenderer : ISkinRenderer<TextureViewer>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;
        private readonly I2DRenderUtilities _renderUtilities;

        public BasicTextureViewerSkinRenderer(IBasicSkinHelper basicSkinHelper, I2DRenderUtilities renderUtilities)
        {
            _basicSkinHelper = basicSkinHelper;
            _renderUtilities = renderUtilities;
        }

        public void Render(IRenderContext renderContext, Rectangle layout, TextureViewer textureViewer)
        {
            _basicSkinHelper.DrawSunken(renderContext, layout);

            if (textureViewer.Texture != null)
            {
                _renderUtilities.RenderTexture(renderContext, new Vector2(layout.X, layout.Y), textureViewer.Texture);
            }
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, TextureViewer container)
        {
            throw new NotSupportedException();
        }
    }
}
