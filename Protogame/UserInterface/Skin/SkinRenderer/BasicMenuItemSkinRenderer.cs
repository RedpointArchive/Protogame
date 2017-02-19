using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicMenuItemSkinRenderer : ISkinRenderer<MenuItem>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetReference<FontAsset> _fontAsset;

        public BasicMenuItemSkinRenderer(IBasicSkinHelper basicSkinHelper, I2DRenderUtilities renderUtilities, IAssetManager assetManager)
        {
            _basicSkinHelper = basicSkinHelper;
            _renderUtilities = renderUtilities;
            _fontAsset = assetManager.Get<FontAsset>("font.Default");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, MenuItem menuItem)
        {
            if (menuItem.Active)
            {
                _basicSkinHelper.DrawRaised(renderContext, layout);
            }
            else
            {
                _basicSkinHelper.DrawFlat(renderContext, layout);
            }

            _renderUtilities.RenderText(
                renderContext,
                new Vector2(layout.X + 5, layout.Center.Y),
                menuItem.Text,
                _fontAsset,
                verticalAlignment: VerticalAlignment.Center);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, MenuItem container)
        {
            return _renderUtilities.MeasureText(
                renderContext,
                text,
                _fontAsset);
        }
    }
}
