using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;

namespace Protogame
{
    public interface ILoadingScreen
    {
        Task WaitForLoadingScreenAssets();

        void Render(IGameContext gameContext, IRenderContext renderContext);

        void RenderEarly(ICoreGame game, SpriteBatch hostLoadedSpriteBatch, Texture2D hostLoadedSplashScreenTexture);
    }
}
