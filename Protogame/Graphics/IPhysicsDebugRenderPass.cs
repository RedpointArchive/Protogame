using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IPhysicsDebugRenderPass : IRenderPass
    {
    }

    public class DefaultPhysicsDebugRenderPass : IPhysicsDebugRenderPass
    {
        private RasterizerState _oldState;

        private RasterizerState _debugState;

        private EffectAsset _basicEffect;

        public DefaultPhysicsDebugRenderPass(IAssetManagerProvider assetManagerProvider)
        {
            _basicEffect = assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.Color");
        }

        public Viewport Viewport { get; set; }

        public Matrix? Projection { get; set; }

        public Matrix? View { get; set; }

        /// <summary>
        /// Gets a value indicating that this is not a post-processing render pass.
        /// </summary>
        /// <value>Always false.</value>
        public bool IsPostProcessingPass
        {
            get { return false; }
        }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            _oldState = renderContext.GraphicsDevice.RasterizerState;

            if (_debugState == null)
            {
                _debugState = new RasterizerState()
                {
                    CullMode = CullMode.None,
                    FillMode = FillMode.WireFrame,
                    DepthBias = _oldState.DepthBias,
                    DepthClipEnable = true,
                    MultiSampleAntiAlias = false,
                    Name = "PhysicsDebugRasterizerState",
                    ScissorTestEnable = false,
                    SlopeScaleDepthBias = _oldState.SlopeScaleDepthBias
                };
            }

            renderContext.GraphicsDevice.RasterizerState = _debugState;
            renderContext.PushEffect(_basicEffect.Effect);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
            renderContext.PopEffect();
            renderContext.GraphicsDevice.RasterizerState = _oldState;
        }
    }
}
