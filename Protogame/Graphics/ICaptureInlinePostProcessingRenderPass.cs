using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// A post-processing render pass which captures the current state
    /// of the render pipeline as a separate render target.  This is
    /// cheaper than <see cref="ICaptureCopyPostProcessingRenderPass"/>, but
    /// you can only access the render target state in the action callback
    /// set on the render pass.  Modifying the render target, e.g. by performing
    /// any rendering at all, will modify the result of the render pipeline.
    /// </summary>
    /// <module>Graphics</module>
    public interface ICaptureInlinePostProcessingRenderPass : IRenderPass
    {
        /// <summary>
        /// A callback that is issued when the render pipeline reaches this
        /// render pass.  This callback makes the current source render
        /// target available for capture.
        /// </summary>
        Action<RenderTarget2D> RenderPipelineStateAvailable { get; set; }
    }
}