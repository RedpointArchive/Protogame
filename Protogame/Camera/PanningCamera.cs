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
        public void Apply(IRenderContext renderContext, Vector2 centerOfScreenPosition)
        {
            if (renderContext.IsCurrentRenderPass<I2DBatchedRenderPass>())
            {
                var batchedRenderPass = renderContext.GetCurrentRenderPass<I2DBatchedRenderPass>();
                batchedRenderPass.RestartWithTransformMatrix(
                    renderContext,
                    Matrix.CreateTranslation(
                        -new Vector3(centerOfScreenPosition, 0)
                        + new Vector3(
                            (batchedRenderPass.Viewport?.Width ?? renderContext.GraphicsDevice.PresentationParameters.BackBufferWidth) / 2,
                            (batchedRenderPass.Viewport?.Height ?? renderContext.GraphicsDevice.PresentationParameters.BackBufferHeight) / 2,
                            0)));
            }
        }
    }
}
