using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="ICanvasRenderPass"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ICanvasRenderPass</interface_ref>
    public class DefaultCanvasRenderPass : ICanvasRenderPass
    {
        public bool IsPostProcessingPass => false;
        public bool SkipWorldRenderBelow => false;
        public bool SkipWorldRenderAbove => false;
        public bool SkipEntityRender => false;
        public bool SkipEngineHookRender => false;
        public string EffectTechniqueName => RenderPipelineTechniqueName.Canvas;

        public Viewport? Viewport { get; set; }

        public SpriteSortMode TextureSortMode
        {
            get;
            set;
        }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            if (Viewport != null)
            {
                renderContext.GraphicsDevice.Viewport = Viewport.Value;
            }
            else
            {
                renderContext.GraphicsDevice.Viewport = new Viewport(
                    0,
                    0,
                    renderContext.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    renderContext.GraphicsDevice.PresentationParameters.BackBufferHeight);
            }

            renderContext.Is3DContext = false;

            renderContext.SpriteBatch.Begin(TextureSortMode);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
            renderContext.SpriteBatch.End();
        }
    }
}

