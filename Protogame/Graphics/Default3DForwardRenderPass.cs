using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The implementation of <see cref="I3DRenderPass"/> which uses forward rendering.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.I3DRenderPass</interface_ref>
    public class Default3DForwardRenderPass : I3DForwardRenderPass
    {
        /// <summary>
        /// Gets a value indicating that this is not a post-processing render pass.
        /// </summary>
        /// <value>Always false.</value>
        public bool IsPostProcessingPass
        {
            get { return false; }
        }

        public string EffectTechniqueName
        {
            get { return RenderPipelineTechniqueName.Forward; }
        }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }
    }
}
