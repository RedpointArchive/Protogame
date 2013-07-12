using System;

namespace Protogame
{
    public interface ICoreGame
    {
        IGameContext GameContext { get; }
        IUpdateContext UpdateContext { get; }
        IRenderContext RenderContext { get; }
    }
}

