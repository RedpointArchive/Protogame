using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicTextBoxSkinRenderer : ISkinRenderer<TextBox>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly FontAsset _fontAsset;

        public BasicTextBoxSkinRenderer(IBasicSkinHelper basicSkinHelper, I2DRenderUtilities renderUtilities, IAssetManagerProvider assetManagerProvider)
        {
            _basicSkinHelper = basicSkinHelper;
            _renderUtilities = renderUtilities;
            _fontAsset = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, TextBox textBox)
        {
            _basicSkinHelper.DrawSunken(renderContext, layout);

            var textToRender = textBox.Text;
            if (textBox.Focused && (textBox.UpdateCounter / 15) % 2 == 0)
            {
                textToRender += "_";
            }

            if (string.IsNullOrEmpty(textBox.Text) && !textBox.Focused)
            {
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(layout.X + 4, layout.Center.Y),
                    textBox.Hint,
                    _fontAsset,
                    textColor: Color.DimGray,
                    verticalAlignment: VerticalAlignment.Center);
            }
            else
            {
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(layout.X + 4, layout.Center.Y),
                    textToRender,
                    _fontAsset,
                    verticalAlignment: VerticalAlignment.Center);
            }
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, TextBox container)
        {
            throw new NotSupportedException();
        }
    }
}
