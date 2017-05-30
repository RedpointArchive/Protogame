using Microsoft.Xna.Framework;
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
        private readonly IBackBufferDimensions _backBufferDimensions;

        public DefaultCanvasRenderPass(
            IBackBufferDimensions backBufferDimensions)
        {
            _backBufferDimensions = backBufferDimensions;

            TextureSortMode = SpriteSortMode.Immediate;
        }

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
                var size = _backBufferDimensions.GetSize(renderContext.GraphicsDevice);
                renderContext.GraphicsDevice.Viewport = new Viewport(
                    0,
                    0,
                    size.X,
                    size.Y);
            }

#if PLATFORM_WINDOWS
            renderContext.World = Matrix.CreateTranslation(0.5f, 0.5f, 0);
#else
            renderContext.World = Matrix.Identity;
#endif
            renderContext.Projection = Matrix.CreateOrthographicOffCenter(
                0,
                renderContext.GraphicsDevice.Viewport.Width,
                renderContext.GraphicsDevice.Viewport.Height,
                0,
                0,
                -1);
            renderContext.View = Matrix.Identity;

            renderContext.SpriteBatch.Begin(TextureSortMode);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
            renderContext.SpriteBatch.End();
        }

        public string Name { get; set; }
    }
}

