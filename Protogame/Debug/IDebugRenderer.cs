using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IDebugRenderer
    {
        void RenderDebugLine(IRenderContext renderContext, Vector3 point1, Vector3 point2, Color color1, Color color2);

        void RenderDebugTriangle(IRenderContext renderContext, Vector3 point1, Vector3 point2, Vector3 point3,
            Color color1, Color color2, Color color3);
    }
}