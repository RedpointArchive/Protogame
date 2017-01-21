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
        private readonly IEffect _invertEffect;

        private readonly IGraphicsBlit _graphicsBlit;

        public DefaultInvertPostProcessingRenderPass(IAssetManagerProvider assetManagerProvider, IGraphicsBlit graphicsBlit)
        {
            _invertEffect = assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.Invert").Effect;
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
            _graphicsBlit.Blit(renderContext, postProcessingSource, null, _invertEffect);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }

        public string Name { get; set; }
    }
}
