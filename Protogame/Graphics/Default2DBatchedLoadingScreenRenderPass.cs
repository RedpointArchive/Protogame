using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="I2DBatchedLoadingScreenRenderPass"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.I2DBatchedRenderPass</interface_ref>
    public class Default2DBatchedLoadingScreenRenderPass : I2DBatchedLoadingScreenRenderPass
    {
        public bool IsPostProcessingPass => false;
        public bool SkipWorldRenderBelow => false;
        public bool SkipWorldRenderAbove => false;
        public bool SkipEntityRender => false;
        public bool SkipEngineHookRender => false;
        public string EffectTechniqueName => RenderPipelineTechniqueName.Batched2D;

        public Viewport? Viewport { get; set; }

        public SpriteSortMode TextureSortMode
        {
            get;
            set;
        }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            renderContext.SpriteBatch.Begin(TextureSortMode);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
            renderContext.SpriteBatch.End();
        }

        public string Name { get; set; }
    }
}

