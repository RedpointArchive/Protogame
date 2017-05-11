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
        public RenderTarget2D UpdateRenderTarget(RenderTarget2D renderTarget, IGameContext gameContext)
        {
            if (IsRenderTargetOutOfDate(renderTarget, gameContext))
            {
                if (renderTarget != null)
                {
                    renderTarget.Dispose();
                }

                if (gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth == 0 &&
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight == 0)
                {
                    return null;
                }

                renderTarget = new RenderTarget2D(
                    gameContext.Graphics.GraphicsDevice,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
                    false,
                    GetRealBackBufferFormat(gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat),
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
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

        public RenderTarget2D UpdateCustomRenderTarget(RenderTarget2D renderTarget, IGameContext gameContext, SurfaceFormat? surfaceFormat, DepthFormat? depthFormat, int? multiSampleCount)
        {
            if (IsCustomRenderTargetOutOfDate(renderTarget, gameContext, surfaceFormat, depthFormat, multiSampleCount))
            {
                if (renderTarget != null)
                {
                    renderTarget.Dispose();
                }

                if (gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth == 0 &&
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight == 0)
                {
                    return null;
                }

                renderTarget = new RenderTarget2D(
                    gameContext.Graphics.GraphicsDevice,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
                    false,
                    surfaceFormat ?? GetRealBackBufferFormat(gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat),
                    depthFormat ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    multiSampleCount ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.PreserveContents);
            }

            return renderTarget;
        }

        public bool IsRenderTargetOutOfDate(RenderTarget2D renderTarget, IGameContext gameContext)
        {
            if (renderTarget == null)
            {
                return true;
            }
            else
            {
                if (renderTarget.Width != gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth)
                {
                    return true;
                }

                if (renderTarget.Height != gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight)
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
                if (renderTarget.Width != gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth)
                {
                    return true;
                }

                if (renderTarget.Height != gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight)
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
