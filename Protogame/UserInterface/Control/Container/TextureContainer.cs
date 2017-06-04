using Microsoft.Xna.Framework;

namespace Protogame
{
    public class TextureContainer : IContainer
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetReference<TextureAsset> _textureAsset;
        private readonly string _textureFit;

        public TextureContainer(I2DRenderUtilities renderUtilities, IAssetReference<TextureAsset> textureAsset, string textureFit)
        {
            _renderUtilities = renderUtilities;
            _textureAsset = textureAsset;
            _textureFit = textureFit;
        }

        public IContainer[] Children => IContainerConstant.EmptyContainers;

        public bool Focused { get; set; }

        public int Order { get; set; }

        public IContainer Parent { get; set; }

        public object Userdata { get; set; }

        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            if (!_textureAsset.IsReady)
            {
                return;
            }

            float x = layout.X;
            float y = layout.Y;
            float width = layout.Width;
            float height = layout.Height;

            switch (_textureFit)
            {
                case "actual":
                    width = _textureAsset.Asset.OriginalWidth;
                    height = _textureAsset.Asset.OriginalHeight;
                    x += (layout.Width - _textureAsset.Asset.OriginalWidth) / 2;
                    y += (layout.Height - _textureAsset.Asset.OriginalHeight) / 2;
                    break;
                case "ratio":
                    var layoutRatio = layout.Width / (float)layout.Height;
                    var textureRatio = _textureAsset.Asset.OriginalWidth / (float)_textureAsset.Asset.OriginalHeight;
                    if (layoutRatio > textureRatio)
                    {
                        // Fit to height.
                        width = layout.Height * textureRatio;
                        x += (layout.Width - width) / 2;
                    }
                    else
                    {
                        // Fit to width.
                        height = layout.Width / textureRatio;
                        y += (layout.Height - height) / 2;
                    }
                    break;
                case "stretch":
                default:
                    // Nothing to do.
                    break;
            }

            _renderUtilities.RenderTexture(
                context,
                new Vector2(x, y),
                _textureAsset,
                new Vector2(width, height));
        }

        public void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }

        public bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event)
        {
            return false;
        }
    }
}
