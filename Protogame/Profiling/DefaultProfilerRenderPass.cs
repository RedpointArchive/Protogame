using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protoinject;

namespace Protogame
{
    public class DefaultProfilerRenderPass : IProfilerRenderPass
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IBackBufferDimensions _backBufferDimensions;
        private readonly IConsole _console;

        public bool IsPostProcessingPass => false;
        public bool SkipWorldRenderBelow => true;
        public bool SkipEntityRender => true;
        public bool SkipWorldRenderAbove => true;
        public bool SkipEngineHookRender => true;
        public string EffectTechniqueName => RenderPipelineTechniqueName.Direct2D;

        public DefaultProfilerRenderPass(
            I2DRenderUtilities renderUtilities,
            IBackBufferDimensions backBufferDimensions,
            [Optional] IConsole console)
        {
            _renderUtilities = renderUtilities;
            _backBufferDimensions = backBufferDimensions;
            _console = console;

            Enabled = true;
            Visualisers = new List<IProfilerVisualiser>();
            Position = ProfilerPosition.TopLeft;
        }

        public bool Enabled { get; set; }

        public List<IProfilerVisualiser> Visualisers { get; }

        public ProfilerPosition Position { get; set; }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass,
            RenderTarget2D postProcessingSource)
        {
            if (!Enabled)
            {
                return;
            }

            var x = 0;
            var y = 0;

            if (_console != null)
            {
                if (_console.State == ConsoleState.Open || _console.State == ConsoleState.OpenNoInput)
                {
                    y = 300;
                }
            }
            
            renderContext.SpriteBatch.Begin(SpriteSortMode.Immediate);

            var size = _backBufferDimensions.GetSize(renderContext.GraphicsDevice);

            foreach (var visualiser in Visualisers)
            {
                var height = visualiser.GetHeight(size.Y);
                var rect = new Rectangle(
                    Position == ProfilerPosition.TopLeft ? 0 : (size.X - 300),
                    y,
                    300,
                    height);

                // Draw a background for the profiler.
                _renderUtilities.RenderRectangle(
                    renderContext,
                    rect,
                    new Color(0, 0, 0, 210),
                    true);

                // Ask the profiler to render it's content.
                visualiser.Render(gameContext, renderContext, rect);

                y += height;
            }

            renderContext.SpriteBatch.End();
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }

        public string Name { get; set; }
    }
}
