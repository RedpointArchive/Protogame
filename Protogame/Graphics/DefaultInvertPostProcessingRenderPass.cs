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
        private readonly IAssetReference<EffectAsset> _invertEffect;

        private readonly IGraphicsBlit _graphicsBlit;

        public DefaultInvertPostProcessingRenderPass(IAssetManager assetManager, IGraphicsBlit graphicsBlit)
        {
            _invertEffect = assetManager.Get<EffectAsset>("effect.Invert");
            _graphicsBlit = graphicsBlit;
        }

        public bool IsPostProcessingPass => true;
        public bool SkipWorldRenderBelow => true;
        public bool SkipWorldRenderAbove => true;
        public bool SkipEntityRender => true;
        public bool SkipEngineHookRender => true;
        public string EffectTechniqueName => RenderPipelineTechniqueName.PostProcess;

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            if (!_invertEffect.IsReady)
            {
                return;
            }

            _graphicsBlit.Blit(renderContext, postProcessingSource, null, _invertEffect.Asset.Effect);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }

        public string Name { get; set; }
    }
}
