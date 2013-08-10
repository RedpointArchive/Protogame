using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class Default3DRenderUtilities : I3DRenderUtilities
    {
        private I2DRenderUtilities m_2DRenderUtilities;
        
        public Default3DRenderUtilities(
            I2DRenderUtilities _2dRenderUtilities)
        {
            this.m_2DRenderUtilities = _2dRenderUtilities;
        }
        
        public void RenderText(
            IRenderContext context,
            Matrix matrix,
            string text,
            FontAsset font,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment verticalAlignment = VerticalAlignment.Top,
            Color? textColor = null,
            bool renderShadow = true,
            Color? shadowColor = null)
        {
            if (!context.Is3DContext) throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            
            var targets = context.GraphicsDevice.GetRenderTargets();
            var size = this.MeasureText(context, text, font);
            
            var temporary = new RenderTarget2D(
                context.GraphicsDevice,
                (int)size.X,
                (int)size.Y,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.DiscardContents);
            context.GraphicsDevice.SetRenderTarget(temporary);
            context.GraphicsDevice.Clear(Color.Transparent);
        
            using (var spriteBatch = new SpriteBatch(context.GraphicsDevice))
            {
                this.m_2DRenderUtilities.RenderText(
                    context,
                    new Vector2(0, 0),
                    text,
                    font,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Top,
                    textColor,
                    renderShadow,
                    shadowColor);
                spriteBatch.End();
            }
            var texture = (Texture2D)temporary;
            context.GraphicsDevice.SetRenderTarget(null);
            
            context.GraphicsDevice.BlendState = BlendState.Opaque;
            context.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            context.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            context.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            
            this.RenderTexture(
                context,
                matrix,
                texture,
                color: Color.White,
                flipHorizontally: false,
                flipVertically: false);
        }
        
        public Vector2 MeasureText(
            IRenderContext context,
            string text,
            FontAsset font)
        {
            return this.m_2DRenderUtilities.MeasureText(
                context,
                text,
                font);
        }
        
        private void RenderTexture(
            IRenderContext context,
            Matrix matrix,
            Texture2D texture,
            Color? color = null,
            bool flipHorizontally = false,
            bool flipVertically = false,
            Rectangle? sourceArea = null)
        {
            if (color != null) throw new NotSupportedException();
            if (flipHorizontally) throw new NotSupportedException();
            if (flipVertically) throw new NotSupportedException();
            if (sourceArea != null) throw new NotSupportedException();
        
            context.EnableTextures();
            context.SetActiveTexture(texture);
            
            var vertexes = new VertexPositionTexture[]
            {
                new VertexPositionTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector2(0, 1)),
                new VertexPositionTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector2(0, 0)),
                new VertexPositionTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector2(1, 1)),
                new VertexPositionTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector2(1, 0))
            };
            var indicies = new short[]{ 1, 3, 0, 2 };
            
            context.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                context.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    vertexes,
                    0,
                    vertexes.Length,
                    indicies,
                    0,
                    vertexes.Length -2 );/// 3);
            }
            context.GraphicsDevice.BlendState = BlendState.Opaque;
        }
        
        public void RenderTexture(
            IRenderContext context,
            Matrix matrix,
            TextureAsset texture,
            Color? color = null,
            bool flipHorizontally = false,
            bool flipVertically = false,
            Rectangle? sourceArea = null)
        {
            if (!context.Is3DContext) throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            
            this.RenderTexture(
                context,
                matrix,
                texture.Texture,
                color: color,
                flipHorizontally: flipHorizontally,
                flipVertically: flipVertically,
                sourceArea: sourceArea);
        }
        
        public void RenderLine(
            IRenderContext context,
            Vector3 start,
            Vector3 end,
            Color color)
        {
            if (!context.Is3DContext) throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            
            context.EnableVertexColors();
            
            var vertexes = new VertexPositionColor[]
            {
                new VertexPositionColor(start, color),
                new VertexPositionColor(end, color)
            };
            var indicies = new short[]{ 0, 1 };
            
            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                context.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    vertexes,
                    0,
                    2,
                    indicies,
                    0,
                    1);
            }
        }
        
        public void RenderRectangle(
            IRenderContext context,
            Vector3 start,
            Vector3 end,
            Color color,
            bool filled = false)
        {
            if (!context.Is3DContext) throw new InvalidOperationException("Can't use 3D rendering utilities in 2D context.");
            
            var vertexes = new VertexPositionColor[]
            {
                new VertexPositionColor(start, color),
                new VertexPositionColor(new Vector3(start.X, end.Y, start.Z), color),
                new VertexPositionColor(new Vector3(end.X, start.Y, end.Z), color),
                new VertexPositionColor(end, color)
            };
            var indicies = new short[]{ 0, 1, 2, 3 };
            
            foreach (var pass in context.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            
                context.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineStrip,
                    vertexes,
                    0,
                    4,
                    indicies,
                    0,
                    3);
            }
        }
    }
}

