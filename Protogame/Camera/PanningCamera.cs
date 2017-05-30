#pragma warning disable CS1591

using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// The default implementation for <see cref="IPanningCamera"/>.
    /// </summary>
    /// <module>Camera</module>
    /// <interface_ref>Protogame.IPanningCamera</interface_ref>
    /// <internal>True</internal>
    public class PanningCamera : IPanningCamera
    {
        private readonly IBackBufferDimensions _backBufferDimensions;

        public PanningCamera(IBackBufferDimensions backBufferDimensions)
        {
            _backBufferDimensions = backBufferDimensions;
        }

        public void Apply(IRenderContext renderContext, Vector2 centerOfScreenPosition)
        {
            if (renderContext.IsCurrentRenderPass<I2DBatchedRenderPass>())
            {
                var size = _backBufferDimensions.GetSize(renderContext.GraphicsDevice);
                var batchedRenderPass = renderContext.GetCurrentRenderPass<I2DBatchedRenderPass>();
                batchedRenderPass.RestartWithTransformMatrix(
                    renderContext,
                    Matrix.CreateTranslation(
                        -new Vector3(centerOfScreenPosition, 0)
                        + new Vector3(
                            (batchedRenderPass.Viewport?.Width ?? size.X) / 2,
                            (batchedRenderPass.Viewport?.Height ?? size.Y) / 2,
                            0)));
            }
        }
    }
}
