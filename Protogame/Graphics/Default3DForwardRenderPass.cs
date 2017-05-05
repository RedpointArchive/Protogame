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
        private readonly IRenderBatcher _renderBatcher;

        public Default3DForwardRenderPass(IRenderBatcher renderBatcher)
        {
            _renderBatcher = renderBatcher;
        }

        public bool IsPostProcessingPass => false;
        public bool SkipWorldRenderBelow => false;
        public bool SkipWorldRenderAbove => false;
        public bool SkipEntityRender => false;
        public bool SkipEngineHookRender => false;
        public string EffectTechniqueName => RenderPipelineTechniqueName.Forward;

        public Viewport? Viewport { get; set; }

        /// <summary>
        /// Clear the depth buffer before this render pass starts rendering.  This allows you to alpha blend
        /// a 3D forward render pass on top of a 2D render pass, without the 2D render pass interfering
        /// with the rendering of 3D objects.
        /// </summary>
        public bool ClearDepthBuffer { get; set; }
        
        public BlendState BlendState { get; set; }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            if (ClearDepthBuffer)
            {
                renderContext.GraphicsDevice.Clear(
                    ClearOptions.DepthBuffer,
                    Microsoft.Xna.Framework.Color.Transparent,
                    renderContext.GraphicsDevice.Viewport.MaxDepth,
                    0);
            }

            if (BlendState != null)
            {
                renderContext.GraphicsDevice.BlendState = BlendState;
            }
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
            _renderBatcher.FlushRequests(gameContext, renderContext);
        }

        public string Name { get; set; }
    }
}
