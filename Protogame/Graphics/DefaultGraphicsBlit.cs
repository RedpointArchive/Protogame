using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// A default implementation of <see cref="IGraphicsBlit"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IGraphicsBlit</interface_ref>
    public class DefaultGraphicsBlit : IGraphicsBlit
    {
        private readonly VertexPositionTexture[] _vertexes =
        {
            new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 0))
        };

        private readonly short[] _indicies = { 1, 3, 0, 2 };

        private readonly Effect _blitEffect;

        private VertexBuffer _vertexBuffer;

        private IndexBuffer _indexBuffer;

        public DefaultGraphicsBlit(IAssetManagerProvider assetManagerProvider)
        {
            _blitEffect = assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.Basic").Effect;
        }

        public void Blit(IRenderContext renderContext, RenderTarget2D source, RenderTarget2D destination = null, Effect shader = null)
        {
            float destWidth, destHeight;
            if (destination != null)
            {
                renderContext.PushRenderTarget(destination);
                destWidth = destination.Width;
                destHeight = destination.Height;
            }
            else
            {
                // TODO: renderContext.GraphicsDevice.GetRenderTargets();
                destWidth = renderContext.GraphicsDevice.PresentationParameters.BackBufferWidth;
                destHeight = renderContext.GraphicsDevice.PresentationParameters.BackBufferHeight;
            }

            if (shader == null)
            {
                shader = _blitEffect;
            }

            if (_vertexBuffer == null)
            {
                _vertexBuffer = new VertexBuffer(renderContext.GraphicsDevice, typeof (VertexPositionTexture),
                    _vertexes.Length, BufferUsage.WriteOnly);
                _vertexBuffer.SetData(_vertexes);
            }

            if (_indexBuffer == null)
            {
                _indexBuffer = new IndexBuffer(renderContext.GraphicsDevice, typeof (short), _indicies.Length,
                    BufferUsage.WriteOnly);
                _indexBuffer.SetData(_indicies);
            }

            var oldWorld = renderContext.World;
            var oldProjection = renderContext.Projection;
            var oldView = renderContext.View;

            renderContext.GraphicsDevice.BlendState = BlendState.Opaque;
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            renderContext.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            renderContext.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            renderContext.World = Matrix.CreateScale(destWidth, destHeight, 1);
            renderContext.Projection = //Matrix.CreateTranslation(-0.5f, -0.5f, 0)* (this might be needed for OpenGL?)
                                       Matrix.CreateOrthographicOffCenter(0, destWidth, destHeight, 0, 0, 1);
            renderContext.View = Matrix.Identity;
            
            renderContext.PushEffect(shader);

            renderContext.EnableTextures();
            renderContext.SetActiveTexture(source);

            renderContext.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            renderContext.GraphicsDevice.Indices = _indexBuffer;
            
            foreach (var pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();

                renderContext.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    0,
                    4,
                    0,
                    2);
            }

            renderContext.PopEffect();

            renderContext.World = oldWorld;
            renderContext.Projection = oldProjection;
            renderContext.View = oldView;

            if (destination != null)
            {
                renderContext.PopRenderTarget();
            }
        }
    }
}
