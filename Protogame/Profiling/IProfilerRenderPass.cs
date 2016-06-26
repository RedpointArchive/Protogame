using System.Collections.Generic;

namespace Protogame
{
    public interface IProfilerRenderPass : IRenderPass
    {
        List<IProfilerVisualiser> Visualisers { get; }
    }
}
