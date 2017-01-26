using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface ILoadingScreen
    {
        void Render(IGameContext gameContext, IRenderContext renderContext);

        void RenderEarly(Game game);
    }
}
