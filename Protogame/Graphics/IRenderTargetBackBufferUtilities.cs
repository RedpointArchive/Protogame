using Microsoft.Xna.Framework;
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
        /// <param name="renderContext">The current render context.</param>
        /// <returns>A render target that matches the backbuffer.</returns>
        RenderTarget2D UpdateRenderTarget(RenderTarget2D renderTarget, IRenderContext renderContext);

        /// <summary>
        /// Given an existing (or null) render target, returns either the existing
        /// render target, or disposes the existing render target and creates a new
        /// one so that the returned render target matches the backbuffer size, with
        /// a custom surface format and no depth buffer.
        /// </summary>
        /// <param name="renderTarget">The existing render target, which is either returned or disposed.</param>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="surfaceFormat">The surface format to use.</param>
        /// <param name="depthFormat"></param>
        /// <param name="multiSampleCount"></param>
        /// <returns>A render target that matches the backbuffer in size.</returns>
        RenderTarget2D UpdateCustomRenderTarget(RenderTarget2D renderTarget, IRenderContext renderContext, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount);

        /// <summary>
        /// Given an existing (or null) render target, returns either the existing
        /// render target, or disposes the existing render target and creates a new
        /// one so that the returned render target matches the specified size and backbuffer.
        /// </summary>
        /// <param name="renderTarget">The existing render target, which is either returned or disposed.</param>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="size">The size of the render target.</param>
        /// <returns>A render target that matches the backbuffer.</returns>
        RenderTarget2D UpdateSizedRenderTarget(RenderTarget2D renderTarget, IRenderContext renderContext, Vector2 size);

        /// <summary>
        /// Given an existing (or null) render target, returns either the existing
        /// render target, or disposes the existing render target and creates a new
        /// one so that the returned render target matches the specified size, with
        /// a custom surface format and no depth buffer.
        /// </summary>
        /// <param name="renderTarget">The existing render target, which is either returned or disposed.</param>
        /// <param name="size">The size of the render target.</param>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="surfaceFormat">The surface format to use.</param>
        /// <param name="depthFormat"></param>
        /// <param name="multiSampleCount"></param>
        /// <param name="shared">If the render target should be shared.</param>
        /// <returns>A render target that matches the backbuffer in size.</returns>
        RenderTarget2D UpdateCustomSizedRenderTarget(RenderTarget2D renderTarget, IRenderContext renderContext, Vector2 size, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount, bool? shared);

        /// <summary>
        /// Returns whether the specified render target matches the backbuffer.
        /// </summary>
        /// <param name="renderTarget">The render target to check.</param>
        /// <param name="renderContext">The current render context.</param>
        /// <returns>Whether the specified render target matches the backbuffer.</returns>
        bool IsRenderTargetOutOfDate(RenderTarget2D renderTarget, IRenderContext renderContext);

        /// <summary>
        /// Returns whether the specified render target matches the backbuffer and size.
        /// </summary>
        /// <param name="renderTarget">The render target to check.</param>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="size">The size of the render target.</param>
        /// <returns>Whether the specified render target matches the backbuffer.</returns>
        bool IsSizedRenderTargetOutOfDate(RenderTarget2D renderTarget, IRenderContext renderContext, Vector2 size);

        /// <summary>
        /// Returns whether the specified custom render target matches the backbuffer size and specified surface format.
        /// </summary>
        /// <param name="renderTarget">The render target to check.</param>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="surfaceFormat">The surface format to use.</param>
        /// <param name="depthFormat"></param>
        /// <param name="multiSampleCount"></param>
        /// <returns>Whether the specified render target matches the backbuffer size and specified surface format.</returns>
        bool IsCustomRenderTargetOutOfDate(RenderTarget2D renderTarget, IRenderContext renderContext, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount);

        /// <summary>
        /// Returns whether the specified custom render target matches the specified size and specified surface format.
        /// </summary>
        /// <param name="renderTarget">The render target to check.</param>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="size">The size of the render target.</param>
        /// <param name="surfaceFormat">The surface format to use.</param>
        /// <param name="depthFormat"></param>
        /// <param name="multiSampleCount"></param>
        /// <returns>Whether the specified render target matches the backbuffer size and specified surface format.</returns>
        bool IsCustomSizedRenderTargetOutOfDate(RenderTarget2D renderTarget, IRenderContext renderContext, Vector2 size, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount);
    }
}
