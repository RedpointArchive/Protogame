using System.Collections.Generic;

namespace Protogame
{
    public interface IDebugRenderPass : IRenderPass
    {
        bool Enabled { get; set; }

        List<VertexPositionNormalColor> Lines { get; }

        List<VertexPositionNormalColor> Triangles { get; }
    }
}
