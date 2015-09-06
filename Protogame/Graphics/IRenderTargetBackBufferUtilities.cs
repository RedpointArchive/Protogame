using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// This services provides utility methods for making
    /// render targets match the backbuffer.
    /// </summary>
    /// <module>Graphics</module>
    public interface IRenderTargetBackBufferUtilities
    {
        /// <summary>
        /// Given an existing (or null) render target, returns either the existing
        /// render target, or disposes the existing render target and creates a new
        /// one so that the returned render target matches the backbuffer.
        /// </summary>
        /// <param name="renderTarget">The existing render target, which is either returned or disposed.</param>
        /// <param name="gameContext">The current game context.</param>
        /// <returns>A render target that matches the backbuffer.</returns>
        RenderTarget2D UpdateRenderTarget(RenderTarget2D renderTarget, IGameContext gameContext);

        /// <summary>
        /// Returns whether the specified render target matches the backbuffer.
        /// </summary>
        /// <param name="renderTarget">The render target to check.</param>
        /// <param name="gameContext">The current game context.</param>
        /// <returns>Whether the specified render target matches the backbuffer.</returns>
        bool IsRenderTargetOutOfDate(RenderTarget2D renderTarget, IGameContext gameContext);
    }
}
