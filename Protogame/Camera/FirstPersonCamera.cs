#pragma warning disable CS1591

using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// The default implementation for <see cref="IFirstPersonCamera"/>.
    /// </summary>
    /// <module>Camera</module>
    /// <interface_ref>Protogame.IFirstPersonCamera</interface_ref>
    /// <internal>True</internal>
    public class FirstPersonCamera : IFirstPersonCamera
    {
        private readonly IBackBufferDimensions _backBufferDimensions;

        public FirstPersonCamera(IBackBufferDimensions backBufferDimensions)
        {
            _backBufferDimensions = backBufferDimensions;
        }

        public void Apply(
            IRenderContext renderContext, 
            Vector3 position,
            Vector3 lookAt, 
            Vector3? up,
            float fieldOfView,
            float nearPlaneDistance,
            float farPlaneDistance)
        {
            var size = _backBufferDimensions.GetSize(renderContext.GraphicsDevice);
            var aspectRatio = size.Width / (float)size.Height;

            up = up ?? Vector3.Up;
            renderContext.CameraPosition = position;
            renderContext.CameraLookAt = lookAt;
            renderContext.View = Matrix.CreateLookAt(position, lookAt, up.Value);
            renderContext.Projection = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView,
                aspectRatio,
                nearPlaneDistance,
                farPlaneDistance);
            renderContext.World = Matrix.Identity;
        }
    }
}
