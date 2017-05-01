using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// A 3D render pass that uses forward rendering.
    /// </summary>
    /// <module>Graphics</module>
    public interface I3DForwardRenderPass : I3DBatchedRenderPass, IRenderPassWithViewport
    {
        /// <summary>
        /// Clear the depth buffer before this render pass starts rendering.  This allows you to alpha blend
        /// a 3D forward render pass on top of a 2D render pass, without the 2D render pass interfering
        /// with the rendering of 3D objects.
        /// </summary>
        bool ClearDepthBuffer { get; set; }

        BlendState BlendState { get; set; }
    }
}
