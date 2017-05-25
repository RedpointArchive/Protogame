using System.Collections.Generic;

namespace Protogame
{
    public interface ILightRenderer<T> : ILightRenderer
    {
    }

    public interface ILightRenderer
    {
        void Render(IGameContext gameContext, IRenderContext renderContext, ILightContext lightContext, IEnumerable<ILight> lights);
    }
}
