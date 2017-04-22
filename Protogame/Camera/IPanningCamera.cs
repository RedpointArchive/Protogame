using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// Given a set of inputs for a 2D panning camera, sets up the rendering matrix
    /// to produce the correct rendering for a 2D panning camera.
    /// </summary>
    public interface IPanningCamera
    {
        /// <summary>
        /// Applies a 2D panning camera to the current rendering context.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="centerOfScreenPosition">The world position which should be displayed in the center of the screen.</param>
        void Apply(
            IRenderContext renderContext,
            Vector2 centerOfScreenPosition);
    }
}
