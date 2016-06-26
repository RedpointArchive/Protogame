using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IProfilerVisualiser
    {
        int GetHeight(int backBufferHeight);

        void Render(IGameContext gameContext, IRenderContext renderContext, Rectangle rectangle);
    }
}
