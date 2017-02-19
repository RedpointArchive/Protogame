﻿using System;
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
using System.Diagnostics;
#endif
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class GCMetricsProfilerVisualiser : IGCMetricsProfilerVisualiser
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetReference<FontAsset> _defaultFont;

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
        private readonly PerformanceCounter _gen0PerformanceCounter;
        private readonly PerformanceCounter _gen1PerformanceCounter;
        private readonly PerformanceCounter _gen2PerformanceCounter;
#endif

        private const string CategoryName = "Process";
        private const string ProcessIdCounter = "ID Process";

        private long _lastGen0Count;
        private long _lastGen1Count;
        private long _lastGen2Count;

        public GCMetricsProfilerVisualiser(
            IAssetManager assetManager,
            I2DRenderUtilities renderUtilities)
        {
            _renderUtilities = renderUtilities;
            _defaultFont = assetManager.Get<FontAsset>("font.Default");

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
            string instanceName;
            if (TryGetInstanceName(Process.GetCurrentProcess(), out instanceName))
            {
                _gen0PerformanceCounter = new PerformanceCounter(".NET CLR Memory", "# Gen 0 Collections",
                    instanceName, true);
                _gen1PerformanceCounter = new PerformanceCounter(".NET CLR Memory", "# Gen 1 Collections",
                    instanceName, true);
                _gen2PerformanceCounter = new PerformanceCounter(".NET CLR Memory", "# Gen 2 Collections",
                    instanceName, true);
            }
#endif
        }

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
        public static bool TryGetInstanceName(Process process, out string instanceName)
        {
            try
            {
                PerformanceCounterCategory processCategory = new PerformanceCounterCategory(CategoryName);
                string[] instanceNames = processCategory.GetInstanceNames();
                foreach (string name in instanceNames)
                {
                    if (name.StartsWith(process.ProcessName))
                    {
                        using (
                            PerformanceCounter processIdCounter = new PerformanceCounter(CategoryName, ProcessIdCounter,
                                name, true))
                        {
                            if (process.Id == (int) processIdCounter.RawValue)
                            {
                                instanceName = name;
                                return true;
                            }
                        }
                    }
                }

                instanceName = null;
                return false;
            }
            catch
            {
                instanceName = null;
                return false;
            }
        }
#endif

        public int GetHeight(int backBufferHeight)
        {
            return 20;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, Rectangle rectangle)
        {
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
            long gen0Value, gen1Value, gen2Value;
            if (_gen0PerformanceCounter == null)
            {
                gen0Value = 0;
            }
            else
            {
                var v = _gen0PerformanceCounter.RawValue;
                gen0Value = v - _lastGen0Count;
                _lastGen0Count = v;
            }
            if (_gen1PerformanceCounter == null)
            {
                gen1Value = 0;
            }
            else
            {
                var v = _gen1PerformanceCounter.RawValue;
                gen1Value = v - _lastGen1Count;
                _lastGen1Count = v;
            }
            if (_gen2PerformanceCounter == null)
            {
                gen2Value = 0;
            }
            else
            {
                var v = _gen2PerformanceCounter.RawValue;
                gen2Value = v - _lastGen2Count;
                _lastGen2Count = v;
            }

            var metrics = new[]
            {
                new Tuple<string, ulong>("gen0", (ulong) gen0Value),
                new Tuple<string, ulong>("gen1", (ulong) gen1Value),
                new Tuple<string, ulong>("gen2", (ulong) gen2Value)
            };
#endif

            _renderUtilities.RenderLine(
                renderContext,
                new Vector2(rectangle.X, rectangle.Y),
                new Vector2(rectangle.X, rectangle.Y + rectangle.Height - 1),
                Color.Red);
            _renderUtilities.RenderLine(
                renderContext,
                new Vector2(rectangle.X + 1, rectangle.Y),
                new Vector2(rectangle.X + 1, rectangle.Y + rectangle.Height - 1),
                Color.Red);

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
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
#else
            _renderUtilities.RenderText(
                renderContext,
                new Vector2(rectangle.X + 5, rectangle.Y),
                "GC stats not available",
                _defaultFont);
#endif
        }
    }
}
