using System.Collections.Generic;

namespace Protogame
{
    public interface IProfilerRenderPass : IRenderPass
    {
        bool Enabled { get; set; }

        List<IProfilerVisualiser> Visualisers { get; }
    }
}
