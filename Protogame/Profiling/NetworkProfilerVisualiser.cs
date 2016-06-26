using Microsoft.Xna.Framework;

namespace Protogame
{
    public class NetworkProfilerVisualiser : INetworkProfilerVisualiser
    {
        public int GetHeight(int backBufferHeight)
        {
            return 200;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, Rectangle rectangle)
        {

        }
    }

    public interface INetworkProfilerVisualiser : IProfilerVisualiser
    {
        
    }
}
