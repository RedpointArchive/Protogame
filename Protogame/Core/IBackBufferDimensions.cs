using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IBackBufferDimensions
    {
        BackBufferSize GetSize(GraphicsDevice graphicsDevice);
    }
}
