using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// A post-processing render pass which captures the current state
    /// of the render pipeline as a separate render target.  This is
    /// more expensive than <see cref="ICaptureInlinePostProcessingRenderPass"/>,
    /// but allows you to access the result at any time between the end of
    /// this render pass, and the begin of this render pass in the next
    /// frame.
    /// </summary>
    /// <module>Graphics</module>
    public interface ICaptureCopyPostProcessingRenderPass : IRenderPass
    {
        /// <summary>
        /// The captured render target.  This property is null before
        /// the first frame is rendered.
        /// </summary>
        RenderTarget2D CapturedRenderTarget { get; }
    }
}