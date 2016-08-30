using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class GraphicsMetricsProfilerVisualiser : IGraphicsMetricsProfilerVisualiser
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IRenderCache _renderCache;
        private readonly IRenderAutoCache _renderAutoCache;
        private readonly IRenderBatcher _renderBatcher;
        private readonly FontAsset _defaultFont;

        public static ulong ParameterSetsCreated = 0;

        public static ulong RenderRequestsCreated = 0;

        public GraphicsMetricsProfilerVisualiser(
            IAssetManagerProvider assetManagerProvider,
            I2DRenderUtilities renderUtilities,
            IRenderCache renderCache,
            IRenderAutoCache renderAutoCache,
            IRenderBatcher renderBatcher)
        {
            _renderUtilities = renderUtilities;
            _renderCache = renderCache;
            _renderAutoCache = renderAutoCache;
            _renderBatcher = renderBatcher;
            _defaultFont = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
        }

        public int GetHeight(int backBufferHeight)
        {
            return 100;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, Rectangle rectangle)
        {
            var graphicsMetrics = renderContext.GraphicsDevice.Metrics;

            var metrics = new[]
            {
                new Tuple<string, ulong>("cc", graphicsMetrics.ClearCount),
                new Tuple<string, ulong>("psc", graphicsMetrics.PixelShaderCount),
                new Tuple<string, ulong>("dc", graphicsMetrics.DrawCount),
                new Tuple<string, ulong>("pc", graphicsMetrics.PrimitiveCount),
                new Tuple<string, ulong>("sc", graphicsMetrics.SpriteCount),
                new Tuple<string, ulong>("tgc", graphicsMetrics.TargetCount),
                new Tuple<string, ulong>("txc", graphicsMetrics.TextureCount),
                new Tuple<string, ulong>("vsc", graphicsMetrics.VertexShaderCount),
                new Tuple<string, ulong>("vbc", (ulong)_renderCache.VertexBuffersCached),
                new Tuple<string, ulong>("ibc", (ulong)_renderCache.IndexBuffersCached),
                new Tuple<string, ulong>("vbac", (ulong)_renderAutoCache.VertexBuffersCached),
                new Tuple<string, ulong>("ibac", (ulong)_renderAutoCache.IndexBuffersCached),
                new Tuple<string, ulong>("bch", _renderBatcher.LastBatchCount),
                new Tuple<string, ulong>("apy", _renderBatcher.LastApplyCount),
                new Tuple<string, ulong>("sav", _renderBatcher.LastBatchSaveCount),
                null,
                new Tuple<string, ulong>("pscr", ParameterSetsCreated),
                new Tuple<string, ulong>("rrcr", RenderRequestsCreated),
            };

            _renderUtilities.RenderLine(
                renderContext,
                new Vector2(rectangle.X, rectangle.Y),
                new Vector2(rectangle.X, rectangle.Y + rectangle.Height - 1),
                Color.Green);
            _renderUtilities.RenderLine(
                renderContext,
                new Vector2(rectangle.X + 1, rectangle.Y),
                new Vector2(rectangle.X + 1, rectangle.Y + rectangle.Height - 1),
                Color.Green);

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

            ParameterSetsCreated = 0;
            RenderRequestsCreated = 0;
        }
    }
}
