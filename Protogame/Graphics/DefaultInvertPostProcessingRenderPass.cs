using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="IInvertPostProcessingRenderPass"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IInvertPostProcessingRenderPass</interface_ref>
    public class DefaultInvertPostProcessingRenderPass : IInvertPostProcessingRenderPass
    {
        private readonly Effect _invertEffect;

        private readonly IGraphicsBlit _graphicsBlit;

        public DefaultInvertPostProcessingRenderPass(IAssetManagerProvider assetManagerProvider, IGraphicsBlit graphicsBlit)
        {
            _invertEffect = assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.Invert").Effect;
            _graphicsBlit = graphicsBlit;
        }

        /// <summary>
        /// Gets a value indicating that this is a post-processing render pass.
        /// </summary>
        /// <value>Always true.</value>
        public bool IsPostProcessingPass
        {
            get { return true; }
        }

        public string EffectTechniqueName { get { return RenderPipelineTechniqueName.PostProcess; } }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            _graphicsBlit.Blit(renderContext, postProcessingSource, null, _invertEffect);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }
    }
}
