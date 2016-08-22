using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicTreeItemSkinRenderer : ISkinRenderer<TreeItem>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly FontAsset _fontAsset;

        public BasicTreeItemSkinRenderer(IBasicSkinHelper basicSkinHelper, I2DRenderUtilities renderUtilities, IAssetManagerProvider assetManagerProvider)
        {
            _basicSkinHelper = basicSkinHelper;
            _renderUtilities = renderUtilities;
            _fontAsset = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, TreeItem treeItem)
        {
            if (treeItem.Parent is TreeView)
            {
                var view = treeItem.Parent as TreeView;
                if (view.SelectedItem == treeItem)
                {
                    _basicSkinHelper.DrawRaised(renderContext, layout);
                }
            }

            _renderUtilities.RenderText(
                renderContext,
                new Vector2(layout.X + 5, layout.Center.Y),
                treeItem.Text,
                _fontAsset,
                verticalAlignment: VerticalAlignment.Center);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, TreeItem container)
        {
            throw new NotSupportedException();
        }
    }
}
