using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultBackBufferDimensions : IBackBufferDimensions
    {
        public BackBufferSize GetSize(GraphicsDevice graphicsDevice)
        {
            return new BackBufferSize(
                graphicsDevice.PresentationParameters.BackBufferWidth,
                graphicsDevice.PresentationParameters.BackBufferHeight);
        }
    }
}
