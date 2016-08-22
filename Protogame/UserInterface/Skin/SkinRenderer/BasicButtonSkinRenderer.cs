using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicButtonSkinRenderer : ISkinRenderer<Button>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly FontAsset _fontAsset;

        public BasicButtonSkinRenderer(IBasicSkinHelper basicSkinHelper, I2DRenderUtilities renderUtilities, IAssetManagerProvider assetManagerProvider)
        {
            _basicSkinHelper = basicSkinHelper;
            _renderUtilities = renderUtilities;
            _fontAsset = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, Button button)
        {
            var offset = 0;
            if (button.State == ButtonUIState.Clicked)
            {
                _basicSkinHelper.DrawSunken(renderContext, layout);
                offset = 1;
            }
            else
            {
                _basicSkinHelper.DrawRaised(renderContext, layout);
            }

            _renderUtilities.RenderText(
                renderContext,
                new Vector2(layout.Center.X + offset, layout.Center.Y + offset),
                button.Text,
                _fontAsset,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, Button container)
        {
            throw new NotSupportedException();
        }
    }
}
