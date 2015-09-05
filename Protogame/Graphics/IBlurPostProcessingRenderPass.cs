namespace Protogame
{
    /// <summary>
    /// A post-processing render pass which applies a guassian blur
    /// to the screen.
    /// </summary>
    public interface IBlurPostProcessingRenderPass : IRenderPass
    {
        /// <summary>
        /// The number of iterations of blur to apply.
        /// </summary>
        int Iterations { get; set; }
    }
}