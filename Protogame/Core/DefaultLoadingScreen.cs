using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protoinject;

namespace Protogame
{
    public class DefaultLoadingScreen : ILoadingScreen
    {
        private I2DRenderUtilities _renderUtilities;
        private IAssetManager _assetManager;

        private IAssetReference<FontAsset> _defaultFont;

        public DefaultLoadingScreen(
            I2DRenderUtilities renderUtilities,
            IAssetManager assetManager)
        {
            _renderUtilities = renderUtilities;
            _assetManager = assetManager;
#if PLATFORM_ANDROID
            _defaultFont = assetManager.Get<FontAsset>("font.HiDpi");
#else
            _defaultFont = assetManager.Get<FontAsset>("font.Default");
#endif
        }

        public async Task WaitForLoadingScreenAssets()
        {
            await _defaultFont.WaitUntilReady();
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.IsCurrentRenderPass<I2DBatchedLoadingScreenRenderPass>())
            {
                var bounds = renderContext.GraphicsDevice.PresentationParameters.Bounds;

                _renderUtilities.RenderRectangle(
                    renderContext,
                    bounds,
                    new Color(0x33, 0x33, 0x33, 0xFF),
                    true);
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(bounds.Width - 20, bounds.Height - 20),
                    "Loading...",
                    _defaultFont,
                    HorizontalAlignment.Right,
                    VerticalAlignment.Bottom,
                    Color.White,
                    true,
                    Color.Black);
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(20, bounds.Height - 20),
                    "Made with Protogame (protogame.org)",
                    _defaultFont,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Bottom,
                    new Color(0x66, 0x66, 0x66, 0xFF),
                    false,
                    Color.Black);
            }
        }

        public void RenderEarly(ICoreGame game, SpriteBatch hostLoadedSpriteBatch, Texture2D hostLoadedSplashScreenTexture)
        {
            game.GraphicsDevice.Clear(new Color(0x33, 0x33, 0x33, 0xFF));
        }
    }
}