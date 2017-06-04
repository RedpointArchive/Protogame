using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="I2DDirectRenderPass"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.I2DDirectRenderPass</interface_ref>
    public class Default2DDirectRenderPass : I2DDirectRenderPass
    {
        private readonly IInterlacedBatchingDepthProvider _interlacedBatchingDepthProvider;

        public Default2DDirectRenderPass(IInterlacedBatchingDepthProvider interlacedBatchingDepthProvider)
        {
            _interlacedBatchingDepthProvider = interlacedBatchingDepthProvider;
        }

        public bool IsPostProcessingPass => false;
        public bool SkipWorldRenderBelow => false;
        public bool SkipWorldRenderAbove => false;
        public bool SkipEntityRender => false;
        public bool SkipEngineHookRender => false;
        public string EffectTechniqueName => RenderPipelineTechniqueName.Direct2D;

        public Viewport? Viewport
        {
            get;
            set;
        }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            _interlacedBatchingDepthProvider.Reset();

            // Setup the default sprite effect.
            var vp = Viewport ?? renderContext.GraphicsDevice.Viewport;

            Matrix projection;

            // Normal 3D cameras look into the -z direction (z = 1 is in font of z = 0). The
            // sprite batch layer depth is the opposite (z = 0 is in front of z = 1).
            // --> We get the correct matrix with near plane 0 and far plane -1.
            Matrix.CreateOrthographicOffCenter(0, vp.Width, vp.Height, 0, 0, -1, out projection);

            // Some platforms require a half pixel offset to match DX.
#if !PLATFORM_WINDOWS
            projection.M41 += -0.5f * projection.M11;
            projection.M42 += -0.5f * projection.M22;
#endif
            
            renderContext.View = Matrix.Identity;
            renderContext.Projection = projection;
            renderContext.World = Matrix.Identity;
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }

        public string Name { get; set; }
    }
}

