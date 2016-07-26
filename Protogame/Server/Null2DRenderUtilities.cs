using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class Null2DRenderUtilities : I2DRenderUtilities
    {
        public Vector2 MeasureText(IRenderContext context, string text, FontAsset font)
        {
            throw new NotSupportedException();
        }

        public void RenderLine(IRenderContext context, Vector2 start, Vector2 end, Color color, float width = 1)
        {
            throw new NotSupportedException();
        }

        public void RenderRectangle(IRenderContext context, Rectangle rectangle, Color color, bool filled = false)
        {
            throw new NotSupportedException();
        }

        public void RenderText(IRenderContext context, Vector2 position, string text, FontAsset font,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment verticalAlignment = VerticalAlignment.Top, Color? textColor = null, bool renderShadow = true,
            Color? shadowColor = null)
        {
            throw new NotSupportedException();
        }

        public void RenderTexture(IRenderContext context, Vector2 position, TextureAsset texture, Vector2? size = null,
            Color? color = null, float rotation = 0, Vector2? rotationAnchor = null, bool flipHorizontally = false, bool flipVertically = false,
            Rectangle? sourceArea = null)
        {
            throw new NotSupportedException();
        }

        public void RenderTexture(IRenderContext context, Vector2 position, Texture2D texture, Vector2? size = null,
            Color? color = null, float rotation = 0, Vector2? rotationAnchor = null, bool flipHorizontally = false, bool flipVertically = false,
            Rectangle? sourceArea = null)
        {
            throw new NotSupportedException();
        }

        public void RenderCircle(IRenderContext context, Vector2 center, int radius, Color color, bool filled = false)
        {
            throw new NotSupportedException();
        }

        public void SuspendSpriteBatch(IRenderContext renderContext)
        {
            throw new NotSupportedException();
        }

        public void ResumeSpriteBatch(IRenderContext renderContext)
        {
            throw new NotSupportedException();
        }
    }
}