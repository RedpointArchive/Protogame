using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Null3DRenderUtilities : I3DRenderUtilities
    {
        public Vector2 MeasureText(IRenderContext context, string text, IAssetReference<FontAsset> font)
        {
            throw new NotSupportedException();
        }

        public void RenderLine(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Vector3 start,
            Vector3 end, Color color)
        {
            throw new NotSupportedException();
        }

        public void RenderLine(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Vector3 start,
            Vector3 end, TextureAsset texture, Vector2 startUV, Vector2 endUV)
        {
            throw new NotSupportedException();
        }

        public void RenderRectangle(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Vector3 start,
            Vector3 end, Color color, bool filled = false)
        {
            throw new NotSupportedException();
        }

        public void RenderText(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix matrix,
            string text, IAssetReference<FontAsset> font, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment verticalAlignment = VerticalAlignment.Top, Color? textColor = null, bool renderShadow = true,
            Color? shadowColor = null)
        {
            throw new NotSupportedException();
        }

        public void RenderTexture(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix matrix,
            IAssetReference<TextureAsset> texture, Color? color = null, bool flipHorizontally = false, bool flipVertically = false,
            Rectangle? sourceArea = null)
        {
            throw new NotSupportedException();
        }

        public void RenderCube(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform,
            Color color)
        {
            throw new NotSupportedException();
        }

        public void RenderCube(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform,
            TextureAsset texture, Vector2 topLeftUV, Vector2 bottomRightUV)
        {
            throw new NotSupportedException();
        }

        public void RenderPlane(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform,
            Color color)
        {
            throw new NotSupportedException();
        }

        public void RenderPlane(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform,
            TextureAsset texture, Vector2 topLeftUV, Vector2 bottomRightUV)
        {
            throw new NotSupportedException();
        }

        public void RenderCircle(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix matrix,
            Vector2 center, int radius, Color color, bool filled = false)
        {
            throw new NotSupportedException();
        }
    }
}