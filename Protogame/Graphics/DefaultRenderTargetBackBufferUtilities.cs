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

        public RenderTarget2D UpdateRenderTarget(RenderTarget2D renderTarget, IGameContext gameContext)
        {
            if (IsRenderTargetOutOfDate(renderTarget, gameContext))
            {
                if (renderTarget != null)
                {
                    renderTarget.Dispose();
                }

                var size = _backBufferDimensions.GetSize(gameContext.Graphics.GraphicsDevice);

                if (size.X == 0 && size.Y == 0)
                {
                    return null;
                }

                renderTarget = new RenderTarget2D(
                    gameContext.Graphics.GraphicsDevice,
                    size.X,
                    size.Y,
                    false,
                    GetRealBackBufferFormat(gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat),
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.PreserveContents);
            }

            return renderTarget;
        }
        public RenderTarget2D UpdateSizedRenderTarget(RenderTarget2D renderTarget, IGameContext gameContext, Vector2 size)
        {
            if (IsSizedRenderTargetOutOfDate(renderTarget, gameContext, size))
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
                    gameContext.Graphics.GraphicsDevice,
                    (int)size.X,
                    (int)size.Y,
                    false,
                    GetRealBackBufferFormat(gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat),
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.PreserveContents);
            }

            return renderTarget;
        }

        public RenderTarget2D UpdateCustomRenderTarget(RenderTarget2D renderTarget, IGameContext gameContext, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount)
        {
            if (IsCustomRenderTargetOutOfDate(renderTarget, gameContext, surfaceFormat, depthFormat, multiSampleCount))
            {
                if (renderTarget != null)
                {
                    renderTarget.Dispose();
                }

                var size = _backBufferDimensions.GetSize(gameContext.Graphics.GraphicsDevice);

                if (size.X == 0 && size.Y == 0)
                {
                    return null;
                }

                renderTarget = new RenderTarget2D(
                    gameContext.Graphics.GraphicsDevice,
                    size.X,
                    size.Y,
                    false,
                    surfaceFormat ?? GetRealBackBufferFormat(gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat),
                    depthFormat ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    multiSampleCount ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.PreserveContents);
            }

            return renderTarget;
        }

        public RenderTarget2D UpdateCustomSizedRenderTarget(RenderTarget2D renderTarget, IGameContext gameContext, Vector2 size, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount)
        {
            if (IsCustomSizedRenderTargetOutOfDate(renderTarget, gameContext, size, surfaceFormat, depthFormat, multiSampleCount))
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
                    gameContext.Graphics.GraphicsDevice,
                    (int)size.X,
                    (int)size.Y,
                    false,
                    surfaceFormat ?? GetRealBackBufferFormat(gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat),
                    depthFormat ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    multiSampleCount ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.PreserveContents);
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

        public bool IsRenderTargetOutOfDate(RenderTarget2D renderTarget, IGameContext gameContext)
        {
            if (renderTarget == null)
            {
                return true;
            }
            else
            {
                var size = _backBufferDimensions.GetSize(gameContext.Graphics.GraphicsDevice);

                if (renderTarget.Width != size.X)
                {
                    return true;
                }

                if (renderTarget.Height != size.Y)
                {
                    return true;
                }

                if (renderTarget.Format != GetRealBackBufferFormat(gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat))
                {
                    return true;
                }

                if (renderTarget.DepthStencilFormat != gameContext.Graphics.GraphicsDevice.PresentationParameters.DepthStencilFormat)
                {
                    return true;
                }

                if (renderTarget.MultiSampleCount != gameContext.Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsSizedRenderTargetOutOfDate(RenderTarget2D renderTarget, IGameContext gameContext, Vector2 size)
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

                if (renderTarget.Format != GetRealBackBufferFormat(gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat))
                {
                    return true;
                }

                if (renderTarget.DepthStencilFormat != gameContext.Graphics.GraphicsDevice.PresentationParameters.DepthStencilFormat)
                {
                    return true;
                }

                if (renderTarget.MultiSampleCount != gameContext.Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsCustomRenderTargetOutOfDate(RenderTarget2D renderTarget, IGameContext gameContext, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount)
        {
            if (renderTarget == null)
            {
                return true;
            }
            else
            {
                var size = _backBufferDimensions.GetSize(gameContext.Graphics.GraphicsDevice);

                if (renderTarget.Width != size.X)
                {
                    return true;
                }

                if (renderTarget.Height != size.Y)
                {
                    return true;
                }

                if (renderTarget.Format != (surfaceFormat ?? GetRealBackBufferFormat(gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat)))
                {
                    return true;
                }

                if (renderTarget.DepthStencilFormat != (depthFormat ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.DepthStencilFormat))
                {
                    return true;
                }

                if (renderTarget.MultiSampleCount != (multiSampleCount ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsCustomSizedRenderTargetOutOfDate(RenderTarget2D renderTarget, IGameContext gameContext, Vector2 size, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount)
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

                if (renderTarget.Format != (surfaceFormat ?? GetRealBackBufferFormat(gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat)))
                {
                    return true;
                }

                if (renderTarget.DepthStencilFormat != (depthFormat ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.DepthStencilFormat))
                {
                    return true;
                }

                if (renderTarget.MultiSampleCount != (multiSampleCount ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
