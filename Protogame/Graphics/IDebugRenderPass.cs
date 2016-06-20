using System.Collections.Generic;

namespace Protogame
{
    public interface IDebugRenderPass : IRenderPass
    {
        List<VertexPositionNormalColor> Lines { get; }

        List<VertexPositionNormalColor> Triangles { get; }
    }
}
