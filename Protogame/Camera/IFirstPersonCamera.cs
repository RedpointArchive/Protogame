using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// Given a set of inputs for an FPS camera, sets up the rendering matrix to produce
    /// the correct rendering for a FPS camera.
    /// <para>
    /// This interface does not represent an instance of a camera; it is a utility class
    /// for configuring the rendering context.  To attach a camera to a componentized entity,
    /// use <see cref="FirstPersonCameraComponent"/>.
    /// </para>
    /// </summary>
    /// <module>Camera</module>
    public interface IFirstPersonCamera
    {
        /// <summary>
        /// Applies an FPS perspective to the current rendering context.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="position">The position of the camera in the world.</param>
        /// <param name="lookAt">The position to look at.</param>
        /// <param name="up">The up vector (defaults to <see cref="Vector3.Up"/>.</param>
        /// <param name="fieldOfView">The field of view in radians (defaults to 75 degrees in radians).</param>
        /// <param name="nearPlaneDistance">The near plane distance (defaults to 0.1).</param>
        /// <param name="farPlaneDistance">The far plane distance (defaults to 1000).</param>
        void Apply(
            IRenderContext renderContext,
            Vector3 position, 
            Vector3 lookAt, 
            Vector3? up = null,
            float fieldOfView = (MathHelper.PiOver2 / 90 * 75f), 
            float nearPlaneDistance = 0.1f, 
            float farPlaneDistance = 1000);
    }
}