using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class PhysicsMetricsProfilerVisualiser : IPhysicsMetricsProfilerVisualiser
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IPhysicsEngine _physicsEngine;
        private readonly IAssetReference<FontAsset> _defaultFont;

        public PhysicsMetricsProfilerVisualiser(
            IAssetManager assetManager,
            I2DRenderUtilities renderUtilities,
            IPhysicsEngine physicsEngine)
        {
            _renderUtilities = renderUtilities;
            _physicsEngine = physicsEngine;
            _defaultFont = assetManager.Get<FontAsset>("font.Default");
        }

        public int GetHeight(int backBufferHeight)
        {
            return 40;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, Rectangle rectangle)
        {
            var physicsMetrics = _physicsEngine.GetPhysicsMetrics();

            var metrics = new[]
            {
                new Tuple<string, ulong>("sio", (ulong) physicsMetrics.StaticImmovableObjects),
                new Tuple<string, ulong>("po", (ulong) physicsMetrics.PhysicsObjects),
                null,
                null,
                new Tuple<string, ulong>("fr", (ulong) (physicsMetrics.SyncFromPhysicsTime*1000)),
                new Tuple<string, ulong>("st", (ulong) (physicsMetrics.PhysicsStepTime*1000)),
                new Tuple<string, ulong>("to", (ulong) (physicsMetrics.SyncToPhysicsTime*1000)),
            };

            _renderUtilities.RenderLine(
                renderContext,
                new Vector2(rectangle.X, rectangle.Y),
                new Vector2(rectangle.X, rectangle.Y + rectangle.Height - 1),
                Color.Cyan);
            _renderUtilities.RenderLine(
                renderContext,
                new Vector2(rectangle.X + 1, rectangle.Y),
                new Vector2(rectangle.X + 1, rectangle.Y + rectangle.Height - 1),
                Color.Cyan);

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
                    renderShadow: false);
            }
        }
    }
}
