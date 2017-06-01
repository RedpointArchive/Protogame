using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="ICaptureCopyPostProcessingRenderPass"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ICaptureCopyPostProcessingRenderPass</interface_ref>
    public class DefaultCaptureCopyPostProcessingRenderPass : ICaptureCopyPostProcessingRenderPass
    {
        private readonly IGraphicsBlit _graphicsBlit;

        private readonly IRenderTargetBackBufferUtilities _renderTargetBackBufferUtilities;

        private RenderTarget2D _renderTarget;

        public DefaultCaptureCopyPostProcessingRenderPass(
            IGraphicsBlit graphicsBlit,
            IRenderTargetBackBufferUtilities renderTargetBackBufferUtilities)
        {
            _graphicsBlit = graphicsBlit;
            _renderTargetBackBufferUtilities = renderTargetBackBufferUtilities;
        }
        
        public bool IsPostProcessingPass => true;
        public bool SkipWorldRenderBelow => true;
        public bool SkipWorldRenderAbove => true;
        public bool SkipEntityRender => true;
        public bool SkipEngineHookRender => true;
        public string EffectTechniqueName => RenderPipelineTechniqueName.PostProcess;

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass,
            RenderTarget2D postProcessingSource)
        {
            _renderTarget = _renderTargetBackBufferUtilities.UpdateRenderTarget(_renderTarget, renderContext);

            // Blit to the capture target.
            _graphicsBlit.Blit(renderContext, postProcessingSource, _renderTarget);

            // Blit to the output.
            _graphicsBlit.Blit(renderContext, postProcessingSource);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }

        public string Name { get; set; }

        public RenderTarget2D CapturedRenderTarget {
            get { return _renderTarget; }
        }
    }
}
