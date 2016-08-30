using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicLabelSkinRenderer : ISkinRenderer<Label>
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly ILayoutPosition _layoutPosition;
        private readonly IAssetReference<FontAsset> _fontAsset;

        public BasicLabelSkinRenderer(
            I2DRenderUtilities renderUtilities, 
            IAssetManager assetManager,
            ILayoutPosition layoutPosition)
        {
            _renderUtilities = renderUtilities;
            _layoutPosition = layoutPosition;
            _fontAsset = assetManager.Get<FontAsset>("font.Default");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, Label label)
        {
            _renderUtilities.RenderText(
                renderContext,
                _layoutPosition.GetPositionInLayout(layout, label.HorizontalAlignment ?? HorizontalAlignment.Center, label.VerticalAlignment ?? VerticalAlignment.Center),
                label.Text,
                label.Font ?? _fontAsset,
                label.HorizontalAlignment ?? HorizontalAlignment.Center,
                label.VerticalAlignment ?? VerticalAlignment.Center,
                label.TextColor,
                label.RenderShadow ?? true,
                label.ShadowColor);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, Label container)
        {
            throw new NotSupportedException();
        }
    }
}
