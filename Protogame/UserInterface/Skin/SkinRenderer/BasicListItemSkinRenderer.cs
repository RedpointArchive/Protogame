using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicListItemSkinRenderer : ISkinRenderer<ListItem>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetReference<FontAsset> _fontAsset;

        public BasicListItemSkinRenderer(IBasicSkinHelper basicSkinHelper, I2DRenderUtilities renderUtilities, IAssetManager assetManager)
        {
            _basicSkinHelper = basicSkinHelper;
            _renderUtilities = renderUtilities;
            _fontAsset = assetManager.Get<FontAsset>("font.Default");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, ListItem listItem)
        {
            if (listItem.Parent is ListView)
            {
                var view = listItem.Parent as ListView;
                if (view.SelectedItem == listItem)
                {
                    _basicSkinHelper.DrawRaised(renderContext, layout);
                }
            }

            _renderUtilities.RenderText(
                renderContext,
                new Vector2(layout.X + 5, layout.Center.Y),
                listItem.Text,
                _fontAsset,
                verticalAlignment: VerticalAlignment.Center);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, ListItem container)
        {
            throw new NotSupportedException();
        }
    }
}
