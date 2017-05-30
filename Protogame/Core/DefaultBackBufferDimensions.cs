using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultBackBufferDimensions : IBackBufferDimensions
    {
        public Point GetSize(GraphicsDevice graphicsDevice)
        {
            return new Point(
                graphicsDevice.PresentationParameters.BackBufferWidth,
                graphicsDevice.PresentationParameters.BackBufferHeight);
        }
    }
}
