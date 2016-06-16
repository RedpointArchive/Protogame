using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IDebugRenderPass : IRenderPass
    {
        List<VertexPositionNormalColor> Lines { get; }

        List<VertexPositionNormalColor> Triangles { get; }
    }
}
