using System;

namespace Protogame
{
    public interface IRenderState : IDisposable
    {
        RenderMode Mode { get; }
    }
}

