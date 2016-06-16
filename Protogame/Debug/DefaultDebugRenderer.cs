using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultDebugRenderer : IDebugRenderer
    {
        public DefaultDebugRenderer()
        {
            
        }

        public void RenderDebugLine(IRenderContext renderContext, Vector3 point1, Vector3 point2, Color color1, Color color2)
        {
            if (renderContext.IsCurrentRenderPass<IDebugRenderPass>())
            {
                var debugRenderPass = renderContext.GetCurrentRenderPass<IDebugRenderPass>();

                debugRenderPass.Lines.Add(new VertexPositionNormalColor(point1, Vector3.Zero, color1));
                debugRenderPass.Lines.Add(new VertexPositionNormalColor(point2, Vector3.Zero, color2));
            }
        }

        public void RenderDebugTriangle(IRenderContext renderContext, Vector3 point1, Vector3 point2, Vector3 point3, Color color1, Color color2, Color color3)
        {
            if (renderContext.IsCurrentRenderPass<IDebugRenderPass>())
            {
                var debugRenderPass = renderContext.GetCurrentRenderPass<IDebugRenderPass>();

                debugRenderPass.Triangles.Add(new VertexPositionNormalColor(point1, Vector3.Zero, color1));
                debugRenderPass.Triangles.Add(new VertexPositionNormalColor(point2, Vector3.Zero, color2));
                debugRenderPass.Triangles.Add(new VertexPositionNormalColor(point3, Vector3.Zero, color3));
            }
        }
    }
}
