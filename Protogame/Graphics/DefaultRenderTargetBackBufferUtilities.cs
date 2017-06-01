using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="IRenderTargetBackBufferUtilities"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IRenderTargetBackBufferUtilities</interface_ref>
    public class DefaultRenderTargetBackBufferUtilities : IRenderTargetBackBufferUtilities
    {
        private readonly IBackBufferDimensions _backBufferDimensions;

        public DefaultRenderTargetBackBufferUtilities(IBackBufferDimensions backBufferDimensions)
        {
            _backBufferDimensions = backBufferDimensions;
        }

        public RenderTarget2D UpdateRenderTarget(RenderTarget2D renderTarget, IRenderContext renderContext)
        {
            if (IsRenderTargetOutOfDate(renderTarget, renderContext))
            {
                if (renderTarget != null)
                {
                    renderTarget.Dispose();
                }

                var size = _backBufferDimensions.GetSize(renderContext.GraphicsDevice);

                if (size.Width == 0 && size.Height == 0)
                {
                    return null;
                }

                renderTarget = new RenderTarget2D(
                    renderContext.GraphicsDevice,
                    size.Width,
                    size.Height,
                    false,
                    GetRealBackBufferFormat(renderContext.GraphicsDevice.PresentationParameters.BackBufferFormat),
                    renderContext.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    renderContext.GraphicsDevice.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.PreserveContents);
            }

            return renderTarget;
        }
        public RenderTarget2D UpdateSizedRenderTarget(RenderTarget2D renderTarget, IRenderContext renderContext, Vector2 size)
        {
            if (IsSizedRenderTargetOutOfDate(renderTarget, renderContext, size))
            {
                if (renderTarget != null)
                {
                    renderTarget.Dispose();
                }

                if (size.X < 1 &&
                    size.Y < 1)
                {
                    return null;
                }

                renderTarget = new RenderTarget2D(
                    renderContext.GraphicsDevice,
                    (int)size.X,
                    (int)size.Y,
                    false,
                    GetRealBackBufferFormat(renderContext.GraphicsDevice.PresentationParameters.BackBufferFormat),
                    renderContext.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    renderContext.GraphicsDevice.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.PreserveContents);
            }

            return renderTarget;
        }

        public RenderTarget2D UpdateCustomRenderTarget(RenderTarget2D renderTarget, IRenderContext renderContext, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount)
        {
            if (IsCustomRenderTargetOutOfDate(renderTarget, renderContext, surfaceFormat, depthFormat, multiSampleCount))
            {
                if (renderTarget != null)
                {
                    renderTarget.Dispose();
                }

                var size = _backBufferDimensions.GetSize(renderContext.GraphicsDevice);

                if (size.Width == 0 && size.Height == 0)
                {
                    return null;
                }

                renderTarget = new RenderTarget2D(
                    renderContext.GraphicsDevice,
                    size.Width,
                    size.Height,
                    false,
                    surfaceFormat ?? GetRealBackBufferFormat(renderContext.GraphicsDevice.PresentationParameters.BackBufferFormat),
                    depthFormat ?? renderContext.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    multiSampleCount ?? renderContext.GraphicsDevice.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.PreserveContents);
            }

            return renderTarget;
        }

        public RenderTarget2D UpdateCustomSizedRenderTarget(RenderTarget2D renderTarget, IRenderContext renderContext, Vector2 size, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount, bool? shared)
        {
            if (IsCustomSizedRenderTargetOutOfDate(renderTarget, renderContext, size, surfaceFormat, depthFormat, multiSampleCount))
            {
                if (renderTarget != null)
                {
                    renderTarget.Dispose();
                }

                if (size.X < 1 ||
                    size.Y < 1)
                {
                    return null;
                }

                renderTarget = new RenderTarget2D(
                    renderContext.GraphicsDevice,
                    (int)size.X,
                    (int)size.Y,
                    false,
                    surfaceFormat ?? GetRealBackBufferFormat(renderContext.GraphicsDevice.PresentationParameters.BackBufferFormat),
                    depthFormat ?? renderContext.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    multiSampleCount ?? renderContext.GraphicsDevice.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.PreserveContents,
                    shared ?? false);

#if PLATFORM_WINDOWS
                if (shared ?? false)
                {
                    renderTarget.AcquireLock(0, 1000000);
                }
#endif
            }

            return renderTarget;
        }

        /// <remarks>
        /// On DirectX platforms, MonoGame silently converts the Color surface format to BGRA32 for
        /// the backbuffer.  In our case, we want our render targets to exactly match the format the
        /// backbuffer is using, so we can use the exact same settings.
        /// </remarks>
        private SurfaceFormat GetRealBackBufferFormat(SurfaceFormat backBufferFormat)
        {
#if PLATFORM_WINDOWS
            return backBufferFormat == SurfaceFormat.Color ? SurfaceFormat.Bgra32 : backBufferFormat;
#else
            return backBufferFormat;
#endif
        }

        public bool IsRenderTargetOutOfDate(RenderTarget2D renderTarget, IRenderContext renderContext)
        {
            if (renderTarget == null)
            {
                return true;
            }
            else
            {
                var size = _backBufferDimensions.GetSize(renderContext.GraphicsDevice);

                if (renderTarget.Width != size.Width)
                {
                    return true;
                }

                if (renderTarget.Height != size.Height)
                {
                    return true;
                }

                if (renderTarget.Format != GetRealBackBufferFormat(renderContext.GraphicsDevice.PresentationParameters.BackBufferFormat))
                {
                    return true;
                }

                if (renderTarget.DepthStencilFormat != renderContext.GraphicsDevice.PresentationParameters.DepthStencilFormat)
                {
                    return true;
                }

                if (renderTarget.MultiSampleCount != renderContext.GraphicsDevice.PresentationParameters.MultiSampleCount)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsSizedRenderTargetOutOfDate(RenderTarget2D renderTarget, IRenderContext renderContext, Vector2 size)
        {
            if (renderTarget == null)
            {
                return true;
            }
            else
            {
                if (renderTarget.Width != size.X)
                {
                    return true;
                }

                if (renderTarget.Height != size.Y)
                {
                    return true;
                }

                if (renderTarget.Format != GetRealBackBufferFormat(renderContext.GraphicsDevice.PresentationParameters.BackBufferFormat))
                {
                    return true;
                }

                if (renderTarget.DepthStencilFormat != renderContext.GraphicsDevice.PresentationParameters.DepthStencilFormat)
                {
                    return true;
                }

                if (renderTarget.MultiSampleCount != renderContext.GraphicsDevice.PresentationParameters.MultiSampleCount)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsCustomRenderTargetOutOfDate(RenderTarget2D renderTarget, IRenderContext renderContext, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount)
        {
            if (renderTarget == null)
            {
                return true;
            }
            else
            {
                var size = _backBufferDimensions.GetSize(renderContext.GraphicsDevice);

                if (renderTarget.Width != size.Width)
                {
                    return true;
                }

                if (renderTarget.Height != size.Height)
                {
                    return true;
                }

                if (renderTarget.Format != (surfaceFormat ?? GetRealBackBufferFormat(renderContext.GraphicsDevice.PresentationParameters.BackBufferFormat)))
                {
                    return true;
                }

                if (renderTarget.DepthStencilFormat != (depthFormat ?? renderContext.GraphicsDevice.PresentationParameters.DepthStencilFormat))
                {
                    return true;
                }

                if (renderTarget.MultiSampleCount != (multiSampleCount ?? renderContext.GraphicsDevice.PresentationParameters.MultiSampleCount))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsCustomSizedRenderTargetOutOfDate(RenderTarget2D renderTarget, IRenderContext renderContext, Vector2 size, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount)
        {
            if (renderTarget == null)
            {
                return true;
            }
            else
            {
                if (renderTarget.Width != size.X)
                {
                    return true;
                }

                if (renderTarget.Height != size.Y)
                {
                    return true;
                }

                if (renderTarget.Format != (surfaceFormat ?? GetRealBackBufferFormat(renderContext.GraphicsDevice.PresentationParameters.BackBufferFormat)))
                {
                    return true;
                }

                if (renderTarget.DepthStencilFormat != (depthFormat ?? renderContext.GraphicsDevice.PresentationParameters.DepthStencilFormat))
                {
                    return true;
                }

                if (renderTarget.MultiSampleCount != (multiSampleCount ?? renderContext.GraphicsDevice.PresentationParameters.MultiSampleCount))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
