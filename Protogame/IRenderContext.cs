using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IRenderContext
    {
        SpriteBatch SpriteBatch { get; }
        Texture2D SingleWhitePixel { get; }
        
        void Render(IGameContext context);
    }
}

