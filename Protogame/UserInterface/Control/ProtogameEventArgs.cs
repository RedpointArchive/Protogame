using System;

namespace Protogame
{
    public class ProtogameEventArgs : EventArgs
    {
        public ProtogameEventArgs(IGameContext gameContext)
        {
            GameContext = gameContext;
        }

        public IGameContext GameContext { get; }
    }
}
