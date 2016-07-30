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
        public bool IsPostProcessingPass => false;
        public bool SkipWorldRenderBelow => false;
        public bool SkipWorldRenderAbove => false;
        public bool SkipEntityRender => false;
        public bool SkipEngineHookRender => false;
        public string EffectTechniqueName => RenderPipelineTechniqueName.Forward;

        public Viewport? Viewport { get; set; }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }
    }
}
