using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicLinkSkinRenderer : ISkinRenderer<Link>
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetReference<FontAsset> _fontAsset;

        public BasicLinkSkinRenderer(I2DRenderUtilities renderUtilities, IAssetManager assetManager)
        {
            _renderUtilities = renderUtilities;
            _fontAsset = assetManager.Get<FontAsset>("font.Default");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, Link link)
        {
            _renderUtilities.RenderText(
                renderContext,
                new Vector2(layout.Center.X, layout.Center.Y),
                link.Text,
                _fontAsset,
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                Color.Blue);
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, Link container)
        {
            throw new NotSupportedException();
        }
    }
}
