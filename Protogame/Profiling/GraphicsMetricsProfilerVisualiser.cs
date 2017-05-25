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
        private readonly IAssetReference<FontAsset> _defaultFont;

        public static ulong ParameterSetsCreated = 0;

        public static ulong RenderRequestsCreated = 0;

        public GraphicsMetricsProfilerVisualiser(
            IAssetManager assetManager,
            I2DRenderUtilities renderUtilities,
            IRenderCache renderCache,
            IRenderAutoCache renderAutoCache,
            IRenderBatcher renderBatcher)
        {
            _renderUtilities = renderUtilities;
            _renderCache = renderCache;
            _renderAutoCache = renderAutoCache;
            _renderBatcher = renderBatcher;
            _defaultFont = assetManager.Get<FontAsset>("font.Default");
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
                new Tuple<string, ulong, ulong>("cc", (ulong)graphicsMetrics.ClearCount, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("psc", (ulong)graphicsMetrics.PixelShaderCount, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("dc", (ulong)graphicsMetrics.DrawCount, 200),
                new Tuple<string, ulong, ulong>("pc", (ulong)graphicsMetrics.PrimitiveCount, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("sc", (ulong)graphicsMetrics.SpriteCount, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("tgc", (ulong)graphicsMetrics.TargetCount, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("txc", (ulong)graphicsMetrics.TextureCount, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("vsc", (ulong)graphicsMetrics.VertexShaderCount, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("vbc", (ulong)_renderCache.VertexBuffersCached, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("ibc", (ulong)_renderCache.IndexBuffersCached, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("vbac", (ulong)_renderAutoCache.VertexBuffersCached, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("ibac", (ulong)_renderAutoCache.IndexBuffersCached, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("bch", _renderBatcher.LastBatchCount, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("apy", _renderBatcher.LastApplyCount, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("sav", _renderBatcher.LastBatchSaveCount, ulong.MaxValue),
                null,
                new Tuple<string, ulong, ulong>("pscr", ParameterSetsCreated, ulong.MaxValue),
                new Tuple<string, ulong, ulong>("rrcr", RenderRequestsCreated, ulong.MaxValue),
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
                    _defaultFont,
                    textColor: metrics[i].Item2 > metrics[i].Item3 ? Color.Red : Color.White,
                    renderShadow: false);
            }

            ParameterSetsCreated = 0;
            RenderRequestsCreated = 0;
        }
    }
}
