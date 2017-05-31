using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class RawTextureContainer : IContainer
    {
        private readonly I2DRenderUtilities _renderUtilities;

        public RawTextureContainer(I2DRenderUtilities renderUtilities)
        {
            _renderUtilities = renderUtilities;
        }

        public Texture2D Texture { get; set; }

        public string TextureFit { get; set; }

        public IContainer[] Children => IContainerConstant.EmptyContainers;

        public bool Focused { get; set; }

        public int Order { get; set; }

        public IContainer Parent { get; set; }

        public object Userdata { get; set; }

        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            if (Texture == null || Texture.IsDisposed)
            {
                return;
            }

            float x = layout.X;
            float y = layout.Y;
            float width = layout.Width;
            float height = layout.Height;

            switch (TextureFit)
            {
                case "actual":
                    width = Texture.Width;
                    height = Texture.Height;
                    x += (layout.Width - Texture.Width) / 2;
                    y += (layout.Height - Texture.Height) / 2;
                    break;
                case "ratio":
                    var layoutRatio = layout.Width / (float)layout.Height;
                    var textureRatio = Texture.Width / (float)Texture.Height;
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
                    return;
            }

            _renderUtilities.RenderTexture(
                context,
                new Vector2(x, y),
                Texture,
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
