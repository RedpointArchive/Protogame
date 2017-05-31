using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IBackBufferDimensions
    {
        Point GetSize(GraphicsDevice graphicsDevice);
    }
}
