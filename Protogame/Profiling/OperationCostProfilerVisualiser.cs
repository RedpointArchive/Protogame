using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class OperationCostProfilerVisualiser : IOperationCostProfilerVisualiser
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IMemoryProfiler _memoryProfiler;
        private readonly FontAsset _defaultFont;

        private Dictionary<string, List<double>> _historyOverTimePeriod;
        private Dictionary<string, double> _maximumOverTimePeriod;
        private Dictionary<string, double> _averageOverTimePeriod;
        private Dictionary<string, int> _lastFrameToHaveData;

        public OperationCostProfilerVisualiser(
            IAssetManagerProvider assetManagerProvider,
            I2DRenderUtilities renderUtilities,
            IMemoryProfiler memoryProfiler)
        {
            _defaultFont = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
            _renderUtilities = renderUtilities;
            _memoryProfiler = memoryProfiler;
            _averageOverTimePeriod = new Dictionary<string, double>();
            _historyOverTimePeriod = new Dictionary<string, List<double>>();
            _lastFrameToHaveData = new Dictionary<string, int>();
            _maximumOverTimePeriod = new Dictionary<string, double>();

            MicrosecondLimit = 14000;
            FramesToAnalyse = 240;
        }

        public int MicrosecondLimit { get; set; }

        public int FramesToAnalyse { get; set; }

        public int GetHeight(int backBufferHeight)
        {
            return (_maximumOverTimePeriod.Count + 1)*20;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, Rectangle rectangle)
        {
            UpdateStatsOverLastSecond(gameContext);

            var y = 2;

            var columnWidth = 70;

            _renderUtilities.RenderText(
                renderContext,
                new Vector2(rectangle.X + rectangle.Width - 2, rectangle.Y + y),
                "max",
                _defaultFont,
                HorizontalAlignment.Right);

            _renderUtilities.RenderText(
                renderContext,
                new Vector2(rectangle.X + rectangle.Width - 2 - columnWidth, rectangle.Y + y),
                "avg",
                _defaultFont,
                HorizontalAlignment.Right);

            y += 20;

            foreach (var entry in _maximumOverTimePeriod.OrderByDescending(k => k.Value))
            {
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(rectangle.X + 2, rectangle.Y + y),
                    entry.Key,
                    _defaultFont);

                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(rectangle.X + rectangle.Width - 2, rectangle.Y + y),
                    Math.Round(entry.Value) + "us",
                    _defaultFont,
                    HorizontalAlignment.Right,
                    textColor: Math.Round(entry.Value) > MicrosecondLimit ? Color.Red : Color.White);

                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(rectangle.X + rectangle.Width - 2 - columnWidth, rectangle.Y + y),
                    Math.Round(_averageOverTimePeriod[entry.Key]) + "us",
                    _defaultFont,
                    HorizontalAlignment.Right,
                    textColor: Math.Round(_averageOverTimePeriod[entry.Key]) > MicrosecondLimit ? Color.Red : Color.White);

                y += 20;
            }

            _memoryProfiler.ResetMeasuredCosts();
        }

        private void UpdateStatsOverLastSecond(IGameContext gameContext)
        {
            var currentTick = gameContext.FrameCount;
            var measuredCosts = _memoryProfiler.GetMeasuredCosts();
            
            foreach (var kv in measuredCosts)
            {
                _lastFrameToHaveData[kv.Key] = currentTick;

                if (!_historyOverTimePeriod.ContainsKey(kv.Key))
                {
                    _historyOverTimePeriod[kv.Key] = new List<double>();
                }

                _historyOverTimePeriod[kv.Key].Add(kv.Value);

                while (_historyOverTimePeriod[kv.Key].Count > FramesToAnalyse)
                {
                    _historyOverTimePeriod[kv.Key].RemoveAt(0);
                }
            }

            foreach (var kv in _lastFrameToHaveData.ToArray())
            {
                if (kv.Value < currentTick - FramesToAnalyse)
                {
                    // Discard old entries.
                    _lastFrameToHaveData.Remove(kv.Key);
                    _historyOverTimePeriod.Remove(kv.Key);
                }
            }

            _maximumOverTimePeriod.Clear();
            _averageOverTimePeriod.Clear();
            foreach (var kv in measuredCosts)
            {
                _maximumOverTimePeriod[kv.Key] = _historyOverTimePeriod[kv.Key].Max();
                _averageOverTimePeriod[kv.Key] = _historyOverTimePeriod[kv.Key].Average();
            }
        }
    }
}

