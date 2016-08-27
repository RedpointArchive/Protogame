// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// An implementation of the debug rendering which is used in headless servers.
    /// </summary>
    /// <remarks>
    /// This is bound by <see cref="ProtogameCoreModule"/> by default, and replaced with an actual
    /// implementation if <see cref="ProtogameDebugModule"/> is loaded.
    /// </remarks>
    /// <module>Debug</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IDebugRenderer</interface_ref>
    public class NullDebugRenderer : IDebugRenderer
    {
        public void RenderDebugLine(IRenderContext renderContext, Vector3 point1, Vector3 point2, Color color1, Color color2)
        {
        }

        public void RenderDebugTriangle(IRenderContext renderContext, Vector3 point1, Vector3 point2, Vector3 point3, Color color1,
            Color color2, Color color3)
        {
        }
    }
}