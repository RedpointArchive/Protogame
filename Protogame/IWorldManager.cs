using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IWorldManager
    {
        void Render<T>(T game) where T : Game, ICoreGame;
        void Update<T>(T game) where T : Game, ICoreGame;
    }
}

