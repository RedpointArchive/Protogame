// ReSharper disable CheckNamespace

using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// An interface which provides useful utilities for rendering debug information
    /// during a debug render pass.
    /// </summary>
    /// <module>Debug</module>
    public interface IDebugRenderer
    {
        /// <summary>
        /// Renders a colored line in absolute space.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="point1">The absolute point to render a line from.</param>
        /// <param name="point2">The absolute point to render a line to.</param>
        /// <param name="color1">The color of the line at the first point.</param>
        /// <param name="color2">The color of the line at the second point.</param>
        void RenderDebugLine(IRenderContext renderContext, Vector3 point1, Vector3 point2, Color color1, Color color2);

        /// <summary>
        /// Renders a colored triangle in absolute space.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="point1">The first point of the triangle.</param>
        /// <param name="point2">The second point of the triangle.</param>
        /// <param name="point3">The third point of the triangle.</param>
        /// <param name="color1">The color of the first point on the triangle.</param>
        /// <param name="color2">The color of the second point on the triangle.</param>
        /// <param name="color3">The color of the third point on the triangle.</param>
        void RenderDebugTriangle(IRenderContext renderContext, Vector3 point1, Vector3 point2, Vector3 point3,
            Color color1, Color color2, Color color3);
    }
}