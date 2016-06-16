using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="ICaptureInlinePostProcessingRenderPass"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ICaptureInlinePostProcessingRenderPass</interface_ref>
    public class DefaultCaptureInlinePostProcessingRenderPass : ICaptureInlinePostProcessingRenderPass
    {
        private readonly IGraphicsBlit _graphicsBlit;
        
        private RenderTarget2D _renderTarget;

        public DefaultCaptureInlinePostProcessingRenderPass(IGraphicsBlit graphicsBlit)
        {
            _graphicsBlit = graphicsBlit;
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
            if (RenderPipelineStateAvailable != null)
            {
                RenderPipelineStateAvailable(postProcessingSource);
            }

            // Blit to the output.
            _graphicsBlit.Blit(renderContext, postProcessingSource);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }
        
        public Action<RenderTarget2D> RenderPipelineStateAvailable { get; set; }
    }
}
