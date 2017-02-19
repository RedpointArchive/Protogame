using System;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class KernelMetricsProfilerVisualiser : IKernelMetricsProfilerVisualiser
    {
        private readonly IKernel _kernel;
        private readonly IHierarchy _hierachy;
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetReference<FontAsset> _defaultFont;

        public KernelMetricsProfilerVisualiser(
            IKernel kernel,
            IHierarchy hierachy,
            IAssetManager assetManager,
            I2DRenderUtilities renderUtilities)
        {
            _kernel = kernel;
            _hierachy = hierachy;
            _renderUtilities = renderUtilities;
            _defaultFont = assetManager.Get<FontAsset>("font.Default");
        }

        public int GetHeight(int backBufferHeight)
        {
            return 20;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, Rectangle rectangle)
        {
            var metrics = new[]
            {
                new Tuple<string, ulong>("rn", (ulong) _hierachy.RootNodes.Count),
                new Tuple<string, ulong>("lco", (ulong) _hierachy.LookupCacheObjectCount)
            };
            
            _renderUtilities.RenderLine(
                renderContext,
                new Vector2(rectangle.X, rectangle.Y),
                new Vector2(rectangle.X, rectangle.Y + rectangle.Height - 1),
                Color.Orange);
            _renderUtilities.RenderLine(
                renderContext,
                new Vector2(rectangle.X + 1, rectangle.Y),
                new Vector2(rectangle.X + 1, rectangle.Y + rectangle.Height - 1),
                Color.Orange);

            for (var i = 0; i < metrics.Length; i++)
            {
                var y = i / 4 * 20;
                var x = i%4*((rectangle.Width - 10) / 4) + 5;

                if (metrics[i] == null)
                {
                    continue;
                }

                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(rectangle.X + x, rectangle.Y + y),
                    metrics[i].Item1 + ": " + metrics[i].Item2,
                    _defaultFont);
            }
        }
    }
}
