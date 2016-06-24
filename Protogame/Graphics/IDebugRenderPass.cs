using System.Collections.Generic;

namespace Protogame
{
    public interface IDebugRenderPass : IRenderPass
    {
        List<IDebugLayer> EnabledLayers { get; }

        List<VertexPositionNormalColor> Lines { get; }

        List<VertexPositionNormalColor> Triangles { get; }
    }
}
