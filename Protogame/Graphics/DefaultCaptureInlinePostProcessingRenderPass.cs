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

        /// <summary>
        /// Gets a value indicating that this is a post-processing render pass.
        /// </summary>
        /// <value>Always true.</value>
        public bool IsPostProcessingPass
        {
            get { return true; }
        }

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
