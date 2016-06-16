using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultConsoleRenderPass : IConsoleRenderPass
    {
        private readonly IConsole _console;

        public bool IsPostProcessingPass => false;
        public bool SkipWorldRenderBelow => true;
        public bool SkipWorldRenderAbove => true;
        public bool SkipEntityRender => true;
        public bool SkipEngineHookRender => true;
        public string EffectTechniqueName => RenderPipelineTechniqueName.Canvas;

        public DefaultConsoleRenderPass(IConsole console)
        {
            _console = console;
        }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass,
            RenderTarget2D postProcessingSource)
        {
            _console.Update(gameContext, ((ICoreGame)gameContext.Game).UpdateContext);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
            renderContext.SpriteBatch.Begin();
            _console.Render(gameContext, renderContext);
            renderContext.SpriteBatch.End();
        }
    }
}