using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicFileSelectSkinRenderer : ISkinRenderer<FileSelect>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetReference<FontAsset> _fontAsset;

        public BasicFileSelectSkinRenderer(IBasicSkinHelper basicSkinHelper, I2DRenderUtilities renderUtilities, IAssetManager assetManager)
        {
            _basicSkinHelper = basicSkinHelper;
            _renderUtilities = renderUtilities;
            _fontAsset = assetManager.Get<FontAsset>("font.Default");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, FileSelect fileSelect)
        {
            var offset = 0;
            if (fileSelect.State == ButtonUIState.Clicked)
            {
                _basicSkinHelper.DrawSunken(renderContext, layout);
                offset = 1;
            }
            else
            {
                _basicSkinHelper.DrawRaised(renderContext, layout);
            }

            var text = fileSelect.Path ?? string.Empty;
            while (text.Length > 0 && _renderUtilities.MeasureText(renderContext, "(file select) ..." + text, _fontAsset).X > layout.Width - 10)
            {
                text = text.Substring(1);
            }

            if (text.Length != (fileSelect.Path ?? string.Empty).Length)
            {
                text = "..." + text;
            }

            _renderUtilities.RenderText(
                renderContext,
                new Vector2(layout.Center.X + offset, layout.Center.Y + offset),
                "(file select) " + text,
                _fontAsset,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, FileSelect container)
        {
            throw new NotSupportedException();
        }
    }
}
