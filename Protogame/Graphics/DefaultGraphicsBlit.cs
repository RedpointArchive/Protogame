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
        private readonly VertexPositionNormalTexture[] _vertexes =
        {
            new VertexPositionNormalTexture(new Vector3(0, 1, 0), Vector3.Zero, new Vector2(0, 1)),
            new VertexPositionNormalTexture(new Vector3(0, 0, 0), Vector3.Zero, new Vector2(0, 0)),
            new VertexPositionNormalTexture(new Vector3(1, 1, 0), Vector3.Zero, new Vector2(1, 1)),
            new VertexPositionNormalTexture(new Vector3(1, 0, 0), Vector3.Zero, new Vector2(1, 0))
        };

        private readonly short[] _indicies = { 1, 3, 0, 2 };

        private readonly IEffect _blitEffect;

        private VertexBuffer _vertexBuffer;

        private IndexBuffer _indexBuffer;

        public DefaultGraphicsBlit(IAssetManagerProvider assetManagerProvider)
        {
            _blitEffect = assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.Texture").Effect;
        }

        public void Blit(IRenderContext renderContext, Texture2D source, RenderTarget2D destination = null, IEffect shader = null, IEffectParameterSet effectParameterSet = null, BlendState blendState = null, Vector2? offset = null, Vector2? size = null)
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

            offset = offset ?? new Vector2(0, 0);
            size = size ?? new Vector2(1 - offset.Value.X, 1 - offset.Value.Y);

            if (blendState == null)
            {
                blendState = BlendState.Opaque;
            }

            if (shader == null)
            {
                shader = _blitEffect;
            }

            if (effectParameterSet == null)
            {
                effectParameterSet = shader.CreateParameterSet();
            }

            if (_vertexBuffer == null)
            {
                _vertexBuffer = new VertexBuffer(renderContext.GraphicsDevice, typeof (VertexPositionNormalTexture),
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

            renderContext.GraphicsDevice.BlendState = blendState;
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            renderContext.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            renderContext.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            renderContext.World = Matrix.CreateScale(destWidth, destHeight, 1);
            renderContext.Projection =
#if !PLATFORM_WINDOWS
                Matrix.CreateTranslation(-0.5f, -0.5f, 0) *
#endif
                Matrix.CreateOrthographicOffCenter(
                    destWidth * (-offset.Value.X / size.Value.X),
                    destWidth * (-offset.Value.X / size.Value.X) + destWidth / size.Value.X, 
                    destHeight * (-offset.Value.Y / size.Value.Y) + destHeight / size.Value.Y,
                    destHeight * (-offset.Value.Y / size.Value.Y),
                    0,
                    1);
            renderContext.View = Matrix.Identity;

            if (source != null && effectParameterSet != null)
            {
                var semantic = effectParameterSet.GetSemantic<ITextureEffectSemantic>();
                if (semantic.Texture != source)
                {
                    semantic.Texture = source;
                }
            }

            renderContext.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            renderContext.GraphicsDevice.Indices = _indexBuffer;
            
            shader.LoadParameterSet(renderContext, effectParameterSet);
            foreach (var pass in shader.NativeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                renderContext.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    0,
                    2);
            }

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
