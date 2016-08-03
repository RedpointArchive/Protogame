namespace Protogame
{
    /// <summary>
    /// A 3D render pass that uses forward rendering.
    /// </summary>
    /// <module>Graphics</module>
    public interface I3DDeferredRenderPass : I3DBatchedRenderPass, IRenderPassWithViewport
    {
        /// <summary>
        /// If set to true, renders the scene as four quadrants with the internal 
        /// render target state.  This can be used to diagnose rendering issues
        /// related to colors, normals, depth map or lighting shaders.
        /// </summary>
        bool DebugGBuffer { get; set; }
    }
}
