using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultColorInImageDetectionDebugRenderer : IColorInImageDetectionDebugRenderer
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly FontAsset _defaultFont;

        public DefaultColorInImageDetectionDebugRenderer(IAssetManager assetManager, I2DRenderUtilities renderUtilities)
        {
            _renderUtilities = renderUtilities;
            _defaultFont = assetManager.Get<FontAsset>("font.Default");
        }

        public void Render(
            IColorInImageDetection colorInImageDetection,
            ISelectedColorHandle selectedColorHandle,
            IRenderContext renderContext,
            Rectangle rectangle)
        {
            var recogArray = colorInImageDetection.GetUnlockedResultsForColor(selectedColorHandle);

            if (recogArray == null)
            {
                return;
            }

            var color = colorInImageDetection.GetValueForColor(selectedColorHandle);

            var widthScale = (int)Math.Ceiling(rectangle.Width /
                                            (float)recogArray.GetLength(0));
            var heightScale = (int)Math.Ceiling(rectangle.Height /
                                             (float)recogArray.GetLength(1));
            var xx = rectangle.X;
            var yy = rectangle.Y;

            for (var x = 0; x < recogArray.GetLength(0); x++)
            {
                for (var y = 0; y < recogArray.GetLength(1); y++)
                {
                    Color col;
                    var score = recogArray[x, y] / colorInImageDetection.GetSensitivityForColor(selectedColorHandle);

                    if (score < 0)
                    {
                        var scoreCapped = Math.Min(255, -score);
                        col = new Color(scoreCapped / 255f, scoreCapped / 255f, scoreCapped / 255f, 1f);
                    }
                    else
                    {
                        var scoreCapped = Math.Max(0, score);
                        col = new Color(
                            scoreCapped / 255f * (color.R / 255f),
                            scoreCapped / 255f * (color.G / 255f),
                            scoreCapped / 255f * (color.B / 255f),
                            1f);
                    }

                    _renderUtilities.RenderRectangle(
                        renderContext,
                        new Rectangle(
                            xx + x * widthScale, yy + y * heightScale, widthScale,heightScale),
                        col, true);
                }
            }

            _renderUtilities.RenderText(
                renderContext,
                new Vector2(
                    rectangle.X + rectangle.Width / 2,
                    rectangle.Y + 40),
                "Total " + colorInImageDetection.GetNameForColor(selectedColorHandle) + ": " + colorInImageDetection.GetTotalDetectedForColor(selectedColorHandle),
                _defaultFont);
        }
    }
}