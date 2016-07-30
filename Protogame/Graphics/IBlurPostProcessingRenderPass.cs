namespace Protogame
{
    /// <summary>
    /// A post-processing render pass which applies a guassian blur
    /// to the screen.
    /// </summary>
    /// <module>Graphics</module>
    public interface IBlurPostProcessingRenderPass : IRenderPass, IRenderPassWithViewport
    {
        /// <summary>
        /// The number of iterations of blur to apply.
        /// </summary>
        int Iterations { get; set; }
    }
}