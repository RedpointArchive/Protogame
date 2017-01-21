using Microsoft.Xna.Framework.Graphics;

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

        /// <summary>
        /// Clear the depth buffer before this render pass starts rendering.  This allows you to alpha blend
        /// a 3D deferred render pass on top of a 2D render pass, without the 2D render pass interfering
        /// with the rendering of 3D objects.
        /// </summary>
        bool ClearDepthBuffer { get; set; }

        /// <summary>
        /// The blend state to use when rendering the final G-buffer onto the backbuffer (or current
        /// render target).  By default this is opaque, which is probably what you want if the deferred
        /// rendering pass is the first in the pipeline.  However if you're rendering 2D content underneath
        /// the 3D content, you should set this to something like <see cref="BlendState.AlphaBlend"/>.
        /// </summary>
        BlendState GBufferBlendState { get; set; }
    }
}
