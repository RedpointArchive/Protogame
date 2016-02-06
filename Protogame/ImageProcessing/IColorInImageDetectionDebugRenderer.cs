using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// An interface which renders the state of a <see cref="IColorInImageDetection"/>
    /// instance, which can be used for debugging or calibration of sensitivity.
    /// </summary>
    public interface IColorInImageDetectionDebugRenderer
    {
        /// <summary>
        /// Renders the state of a <see cref="IColorInImageDetection"/>.
        /// </summary>
        /// <param name="colorInImageDetection">The <see cref="IColorInImageDetection"/> whose state should be rendered.</param>
        /// <param name="selectedColorHandle">The color registered with the <see cref="IColorInImageDetection"/>.</param>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="rectangle">The rectangle to render the state in.</param>
        void Render(
            IColorInImageDetection colorInImageDetection,
            ISelectedColorHandle selectedColorHandle, 
            IRenderContext renderContext,
            Rectangle rectangle);
    }
}