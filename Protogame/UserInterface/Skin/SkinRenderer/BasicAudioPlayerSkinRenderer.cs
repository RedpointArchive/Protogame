using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicAudioPlayerSkinRenderer : ISkinRenderer<AudioPlayer>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetReference<FontAsset> _fontAsset;

        public BasicAudioPlayerSkinRenderer(IBasicSkinHelper basicSkinHelper, I2DRenderUtilities renderUtilities, IAssetManager assetManager)
        {
            _basicSkinHelper = basicSkinHelper;
            _renderUtilities = renderUtilities;
            _fontAsset = assetManager.Get<FontAsset>("font.Default");
        }

        public void Render(IRenderContext renderContext, Rectangle layout, AudioPlayer audioPlayer)
        {
            _basicSkinHelper.DrawSunken(renderContext, layout);

            if (audioPlayer.Audio?.PlatformData != null)
            {
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(layout.Center.X, layout.Center.Y + 12),
                    "No visualization available.",
                    _fontAsset,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center);
            }
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, AudioPlayer container)
        {
            throw new NotSupportedException();
        }
    }
}
