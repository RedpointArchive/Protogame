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

        public void Render(IGameContext gameContext, IRenderContext renderContext, StringBuilder inputBuffer, List<Tuple<ConsoleLogLevel, string>> logEntries)
        {
            if (this._fontAsset == null)
            {
                this._fontAsset = this._assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Console");
            }

            if (renderContext.IsCurrentRenderPass<I3DRenderPass>())
            {
                return;
            }

            this._renderUtilities.RenderRectangle(
                renderContext,
                new Rectangle(0, 0, gameContext.Window.ClientBounds.Width, 300),
                new Color(0, 0, 0, 210),
                true);
            this._renderUtilities.RenderRectangle(
                renderContext,
                new Rectangle(0, 0, gameContext.Window.ClientBounds.Width - 1, 300),
                Color.White);
            this._renderUtilities.RenderLine(
                renderContext,
                new Vector2(0, 300 - 16),
                new Vector2(gameContext.Window.ClientBounds.Width, 300 - 16),
                Color.White);
            this._renderUtilities.RenderText(
                renderContext,
                new Vector2(2, 300 - 16),
                inputBuffer.ToString(),
                this._fontAsset);
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
                this._renderUtilities.RenderText(
                    renderContext,
                    new Vector2(2, 300 - 32 - a * 16),
                    logEntries[logEntries.Count - i - 1].Item2,
                    this._fontAsset,
                    textColor: color);
                a++;
            }
        }
    }
}
