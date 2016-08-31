using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class ClientConsoleRender : IConsoleRender
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetManagerProvider _assetManagerProvider;

        private FontAsset _fontAsset;

        public ClientConsoleRender(I2DRenderUtilities renderUtilities, IAssetManagerProvider assetManagerProvider)
        {
            _renderUtilities = renderUtilities;
            _assetManagerProvider = assetManagerProvider;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, StringBuilder inputBuffer, ConsoleState state, List<Tuple<ConsoleLogLevel, string>> logEntries)
        {
            if (_fontAsset == null)
            {
                _fontAsset = _assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Console");
            }

            if (renderContext.IsCurrentRenderPass<I3DRenderPass>())
            {
                return;
            }

            var h = 0;
            if (state == ConsoleState.Open || state == ConsoleState.OpenNoInput)
            {
                h = 300;
            }
            else if (state == ConsoleState.FullOpen || state == ConsoleState.FullOpenNoInput)
            {
                h = gameContext.Window.ClientBounds.Height;
            }

            _renderUtilities.RenderRectangle(
                renderContext,
                new Rectangle(0, 0, gameContext.Window.ClientBounds.Width, h),
                new Color(0, 0, 0, 210),
                true);
            _renderUtilities.RenderRectangle(
                renderContext,
                new Rectangle(0, 0, gameContext.Window.ClientBounds.Width - 1, h - 1),
                Color.White);

            var o = 16;
            if (state == ConsoleState.FullOpen || state == ConsoleState.Open)
            {
                _renderUtilities.RenderLine(
                    renderContext,
                    new Vector2(0, h - 16),
                    new Vector2(gameContext.Window.ClientBounds.Width, h - 16),
                    Color.White);
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(2, h - 16),
                    inputBuffer.ToString(),
                    _fontAsset);
                o = 32;
            }

            var a = 0;
            for (var i = Math.Max(0, logEntries.Count - 30); i < logEntries.Count; i++)
            {
                var color = Color.White;
                switch (logEntries[logEntries.Count - i - 1].Item1)
                {
                    case ConsoleLogLevel.Debug:
                        color = Color.White;
                        break;
                    case ConsoleLogLevel.Info:
                        color = Color.Cyan;
                        break;
                    case ConsoleLogLevel.Warning:
                        color = Color.Orange;
                        break;
                    case ConsoleLogLevel.Error:
                        color = Color.Red;
                        break;
                }
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(2, h - o - a * 16),
                    logEntries[logEntries.Count - i - 1].Item2,
                    _fontAsset,
                    textColor: color);
                a++;
            }
        }
    }
}
