using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicLabelSkinRenderer : ISkinRenderer<Label>
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly ILayoutPosition _layoutPosition;
        private readonly FontAsset _fontAsset;

        public BasicLabelSkinRenderer(
            I2DRenderUtilities renderUtilities, 
            IAssetManagerProvider assetManagerProvider,
            ILayoutPosition layoutPosition)
        {
            _renderUtilities = renderUtilities;
            _layoutPosition = layoutPosition;
            _fontAsset = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, Label label)
        {
            _renderUtilities.RenderText(
                renderContext,
                _layoutPosition.GetPositionInLayout(layout, label.HorizontalAlignment ?? HorizontalAlignment.Center, label.VerticalAlignment ?? VerticalAlignment.Center),
                label.Text,
                _fontAsset,
                label.HorizontalAlignment ?? HorizontalAlignment.Center,
                label.VerticalAlignment ?? VerticalAlignment.Center,
                label.OverrideColor);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, Label container)
        {
            throw new NotSupportedException();
        }
    }
}
