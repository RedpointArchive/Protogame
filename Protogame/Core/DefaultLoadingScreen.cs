using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class DefaultLoadingScreen : ILoadingScreen
    {
        private readonly IKernel _kernel;

        private I2DRenderUtilities _renderUtilities;
        private IAssetManager _assetManager;

        private IAssetReference<FontAsset> _defaultFont;

        public DefaultLoadingScreen(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (_renderUtilities == null)
            {
                _renderUtilities = _kernel.Get<I2DRenderUtilities>();
            }

            if (_assetManager == null)
            {
                _assetManager = _kernel.Get<IAssetManager>();
            }

            if (_defaultFont == null)
            {
                _defaultFont = _assetManager.Get<FontAsset>("font.Default");
            }

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

        public void RenderEarly(Game game)
        {
            game.GraphicsDevice.Clear(new Color(0x33, 0x33, 0x33, 0xFF));
        }
    }
}