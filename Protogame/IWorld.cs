using System;
using System.Collections.Generic;

namespace Protogame
{
    public interface IWorld
    {
        List<IEntity> Entities { get; }
        void RenderBelow(IGameContext gameContext, IRenderContext renderContext);
        void RenderAbove(IGameContext gameContext, IRenderContext renderContext);
        void Update(IGameContext gameContext, IUpdateContext updateContext);
    }
}

