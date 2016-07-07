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

                renderTarget = new RenderTarget2D(
                    gameContext.Graphics.GraphicsDevice,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
                    false,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
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

                renderTarget = new RenderTarget2D(
                    gameContext.Graphics.GraphicsDevice,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
                    false,
                    surfaceFormat ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
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

                if (renderTarget.Format != gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat)
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

                if (renderTarget.Format != (surfaceFormat ?? gameContext.Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat))
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
