using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="IBlurPostProcessingRenderPass"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IBlurPostProcessingRenderPass</interface_ref>
    public class DefaultBlurPostProcessingRenderPass : IBlurPostProcessingRenderPass
    {
        private readonly IAssetReference<EffectAsset> _blurEffect;

        private readonly IGraphicsBlit _graphicsBlit;

        public DefaultBlurPostProcessingRenderPass(IAssetManager assetManager, IGraphicsBlit graphicsBlit)
        {
            _blurEffect = assetManager.Get<EffectAsset>("effect.Blur");
            _graphicsBlit = graphicsBlit;
        }
        
        public bool IsPostProcessingPass => true;
        public bool SkipWorldRenderBelow => true;
        public bool SkipWorldRenderAbove => true;
        public bool SkipEntityRender => true;
        public bool SkipEngineHookRender => true;
        public string EffectTechniqueName => RenderPipelineTechniqueName.PostProcess;

        public Viewport? Viewport { get; set; }

        /// <summary>
        /// Gets or sets the number of blur iterations to apply.
        /// </summary>
        public int Iterations { get; set; }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            if (!_blurEffect.IsReady)
            {
                return;
            }

            // TODO Make iterations work.

            _blurEffect.Asset.Effect.NativeEffect.Parameters["PixelWidth"].SetValue(1f/postProcessingSource.Width);
            _blurEffect.Asset.Effect.NativeEffect.Parameters["PixelHeight"].SetValue(1f/postProcessingSource.Height);
            //_blurEffect.CurrentTechnique.Passes[0].Apply();

            // Parameters will get applied when blitting occurs.

            _graphicsBlit.Blit(renderContext, postProcessingSource, null, _blurEffect.Asset.Effect);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }

        public string Name { get; set; }
    }
}
