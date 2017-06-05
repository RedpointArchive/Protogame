﻿//#define DISABLE_PIPELINE_TARGETS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="IRenderPipeline"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IRenderPipeline</interface_ref>
    public class DefaultRenderPipeline : IRenderPipeline, IAsynchronouslyConstructable
    {
        private readonly List<IRenderPass> _standardRenderPasses;

        private readonly List<IRenderPass> _postProcessingRenderPasses;

        private readonly List<IRenderPass> _transientStandardRenderPasses;

        private readonly List<IRenderPass> _transientPostProcessingRenderPasses;

        private readonly IGraphicsBlit _graphicsBlit;

        private readonly IRenderTargetBackBufferUtilities _renderTargetBackBufferUtilities;

        private readonly IProfiler _profiler;

        private readonly ILoadingScreen _loadingScreen;

        private readonly IBackBufferDimensions _backBufferDimensions;

        private readonly IEngineHook[] _engineHooks;

        private RenderTarget2D _primary;

        private RenderTarget2D _secondary;

        private IRenderPass _renderPass;

        private bool _isFirstRenderPass;

        private Task<ILoadingScreen> _loadingScreenTask;

        public DefaultRenderPipeline(
            IGraphicsBlit graphicsBlit,
            IRenderTargetBackBufferUtilities renderTargetBackBufferUtilities,
            IProfiler profiler,
            ILoadingScreen loadingScreen,
            IBackBufferDimensions backBufferDimensions,
            [FromGame] IEngineHook[] engineHooks)
        {
            _graphicsBlit = graphicsBlit;
            _renderTargetBackBufferUtilities = renderTargetBackBufferUtilities;
            _profiler = profiler;
            _loadingScreen = loadingScreen;
            _backBufferDimensions = backBufferDimensions;
            _engineHooks = engineHooks;
            _standardRenderPasses = new List<IRenderPass>();
            _postProcessingRenderPasses = new List<IRenderPass>();
            _transientStandardRenderPasses = new List<IRenderPass>();
            _transientPostProcessingRenderPasses = new List<IRenderPass>();
            _renderPass = null;
            _isFirstRenderPass = false;
        }

        public async Task ConstructAsync()
        {
            await _loadingScreen.WaitForLoadingScreenAssets();
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            try
            {
                renderContext.Render(gameContext);

                // NOTE: We MUST clear the depth buffer because OpenGL will not do it for us.
                renderContext.GraphicsDevice.Clear(
                    ClearOptions.DepthBuffer, 
                    Microsoft.Xna.Framework.Color.Transparent, 
                    renderContext.GraphicsDevice.Viewport.MaxDepth,
                    0);

                _primary = _renderTargetBackBufferUtilities.UpdateRenderTarget(_primary, renderContext);
                _secondary = _renderTargetBackBufferUtilities.UpdateRenderTarget(_secondary, renderContext);

                if (_primary == null || _secondary == null)
                {
                    // These are null if the window is minimized or invalid, in which case we don't
                    // render anything anyway.
                    return;
                }

                var standardRenderPasses = _standardRenderPasses.ToArray();
                var postProcessingRenderPasses = _postProcessingRenderPasses.ToArray();
                IRenderPass previousPass = null;
                IRenderPass nextPass = null;

                IEntity[] entities = null;
                ILoadingScreen loadingScreen = null;
                if (gameContext.World == null)
                {
                    loadingScreen = _loadingScreen;
                }
                else
                {
                    entities = gameContext.World.GetEntitiesForWorld(gameContext.Hierarchy).ToArray();
                }

#if !DISABLE_PIPELINE_TARGETS
                renderContext.PushRenderTarget(_primary);
#endif

                for (var i = 0; i < standardRenderPasses.Length; i++)
                {
                    var pass = standardRenderPasses[i];
                    using (_profiler.Measure("r-" + pass.GetType().Name))
                    {
                        _isFirstRenderPass = previousPass == null;
                        _renderPass = pass;
                        SetupRenderPassViewport(renderContext, pass);
                        pass.BeginRenderPass(gameContext, renderContext, previousPass, null);
                        previousPass = pass;
                        RenderPass(gameContext, renderContext, pass, entities, loadingScreen);
                        if (i < standardRenderPasses.Length - 1)
                        {
                            nextPass = standardRenderPasses[i + 1];
                        }
                        else if (_transientStandardRenderPasses.Count > 0)
                        {
                            nextPass = _transientStandardRenderPasses[0];
                        }
                        else if (postProcessingRenderPasses.Length > 0)
                        {
                            nextPass = postProcessingRenderPasses[0];
                        }
                        else if (_transientPostProcessingRenderPasses.Count > 0)
                        {
                            nextPass = _transientPostProcessingRenderPasses[0];
                        }
                        else
                        {
                            nextPass = null;
                        }
                        pass.EndRenderPass(gameContext, renderContext, nextPass);
                    }
                }

                var loop = 100;
                while (_transientStandardRenderPasses.Count > 0 && loop-- >= 0)
                {
                    var transientStandardRenderPasses = _transientStandardRenderPasses.ToArray();
                    _transientStandardRenderPasses.Clear();

                    for (var i = 0; i < transientStandardRenderPasses.Length; i++)
                    {
                        var pass = transientStandardRenderPasses[i];
                        using (_profiler.Measure("r-" + pass.GetType().Name))
                        {
                            _isFirstRenderPass = previousPass == null;
                            _renderPass = pass;
                            SetupRenderPassViewport(renderContext, pass);
                            pass.BeginRenderPass(gameContext, renderContext, previousPass, null);
                            previousPass = pass;
                            RenderPass(gameContext, renderContext, pass, entities, loadingScreen);
                            if (i < transientStandardRenderPasses.Length - 1)
                            {
                                nextPass = transientStandardRenderPasses[i + 1];
                            }
                            else if (_transientStandardRenderPasses.Count > 0)
                            {
                                nextPass = _transientStandardRenderPasses[0];
                            }
                            else if (postProcessingRenderPasses.Length > 0)
                            {
                                nextPass = postProcessingRenderPasses[0];
                            }
                            else if (_transientPostProcessingRenderPasses.Count > 0)
                            {
                                nextPass = _transientPostProcessingRenderPasses[0];
                            }
                            else
                            {
                                nextPass = null;
                            }
                            pass.EndRenderPass(gameContext, renderContext, nextPass);
                        }
                    }
                }
                if (loop < 0)
                {
                    throw new InvalidOperationException(
                        "Exceeded the number of AppendTransientRenderPass iterations (100).  Ensure you " +
                        "are not unconditionally calling AppendTransientRenderPass within another render pass.");
                }

#if !DISABLE_PIPELINE_TARGETS
                renderContext.PopRenderTarget();
#endif

                if (postProcessingRenderPasses.Length == 0 && _transientPostProcessingRenderPasses.Count == 0)
                {
                    // Blit the primary render target to the backbuffer and return.
#if !DISABLE_PIPELINE_TARGETS
                    _graphicsBlit.Blit(renderContext, _primary);
#endif
                    return;
                }

                var currentSource = _primary;
                var currentDest = _secondary;

#if !DISABLE_PIPELINE_TARGETS
                renderContext.PushRenderTarget(currentDest);
#endif

                for (var i = 0; i < postProcessingRenderPasses.Length; i++)
                {
                    var pass = postProcessingRenderPasses[i];
                    using (_profiler.Measure("r-" + pass.GetType().Name))
                    {
                        _isFirstRenderPass = previousPass == null;
                        _renderPass = pass;
                        pass.BeginRenderPass(gameContext, renderContext, previousPass, currentSource);
                        previousPass = pass;
                        if (i < postProcessingRenderPasses.Length - 1)
                        {
                            nextPass = postProcessingRenderPasses[i + 1];
                        }
                        else if (_transientPostProcessingRenderPasses.Count > 0)
                        {
                            nextPass = _transientPostProcessingRenderPasses[0];
                        }
                        else
                        {
                            nextPass = null;
                        }
                        pass.EndRenderPass(gameContext, renderContext, nextPass);

                        var temp = currentSource;
                        currentSource = currentDest;
                        currentDest = temp;

#if !DISABLE_PIPELINE_TARGETS
                        renderContext.PopRenderTarget();
                        renderContext.PushRenderTarget(currentDest);
#endif

                        // NOTE: This does not clear the new destination render target; it is expected that
                        // post-processing effects will fully overwrite the destination.
                    }
                }

                loop = 100;
                while (_transientPostProcessingRenderPasses.Count > 0 && loop-- >= 0)
                {
                    var transientPostProcessingRenderPasses = _transientPostProcessingRenderPasses.ToArray();
                    _transientPostProcessingRenderPasses.Clear();

                    for (var i = 0; i < transientPostProcessingRenderPasses.Length; i++)
                    {
                        var pass = transientPostProcessingRenderPasses[i];
                        using (_profiler.Measure("r-" + pass.GetType().Name))
                        {
                            _isFirstRenderPass = previousPass == null;
                            _renderPass = pass;
                            pass.BeginRenderPass(gameContext, renderContext, previousPass, currentSource);
                            previousPass = pass;
                            if (i < transientPostProcessingRenderPasses.Length - 1)
                            {
                                nextPass = transientPostProcessingRenderPasses[i + 1];
                            }
                            else if (_transientPostProcessingRenderPasses.Count > 0)
                            {
                                nextPass = _transientPostProcessingRenderPasses[0];
                            }
                            else
                            {
                                nextPass = null;
                            }
                            pass.EndRenderPass(gameContext, renderContext, nextPass);

                            var temp = currentSource;
                            currentSource = currentDest;
                            currentDest = temp;

#if !DISABLE_PIPELINE_TARGETS
                            renderContext.PopRenderTarget();
                            renderContext.PushRenderTarget(currentDest);
#endif

                            // NOTE: This does not clear the new destination render target; it is expected that
                            // post-processing effects will fully overwrite the destination.
                        }
                    }
                }
                if (loop < 0)
                {
                    throw new InvalidOperationException(
                        "Exceeded the number of AppendTransientRenderPass iterations (100).  Ensure you " +
                        "are not unconditionally calling AppendTransientRenderPass within another render pass.");
                }

#if !DISABLE_PIPELINE_TARGETS
                renderContext.PopRenderTarget();

                _graphicsBlit.Blit(renderContext, currentSource);
#endif
            }
            finally
            {
                _renderPass = null;
                _isFirstRenderPass = false;
            }
        }

        private void SetupRenderPassViewport(IRenderContext renderContext, IRenderPass pass)
        {
            var renderPassWithViewport = pass as IRenderPassWithViewport;

            Viewport newViewport;
            if (renderPassWithViewport?.Viewport != null)
            {
                newViewport = renderPassWithViewport.Viewport.Value;
            }
            else
            {
                var size = _backBufferDimensions.GetSize(renderContext.GraphicsDevice);
                newViewport = new Viewport(
                    0,
                    0,
                    size.Width,
                    size.Height);
            }

            var currentViewport = renderContext.GraphicsDevice.Viewport;
            if (currentViewport.X != newViewport.X ||
                currentViewport.Y != newViewport.Y ||
                currentViewport.Width != newViewport.Width ||
                currentViewport.Height != newViewport.Height)
            {
                // The viewport is different, assign it to the GPU.
                renderContext.GraphicsDevice.Viewport = newViewport;
            }
        }

        public IRenderPass AddFixedRenderPass(IRenderPass renderPass)
        {
            if (renderPass.IsPostProcessingPass)
            {
                _postProcessingRenderPasses.Add(renderPass);
            }
            else
            {
                _standardRenderPasses.Add(renderPass);
            }

            return renderPass;
        }

        public void RemoveFixedRenderPass(IRenderPass renderPass)
        {
            if (renderPass.IsPostProcessingPass)
            {
                _postProcessingRenderPasses.Remove(renderPass);
            }
            else
            {
                _standardRenderPasses.Remove(renderPass);
            }
        }

        public IRenderPass AppendTransientRenderPass(IRenderPass renderPass)
        {
            if (renderPass.IsPostProcessingPass)
            {
                _transientPostProcessingRenderPasses.Add(renderPass);
            }
            else
            {
                _transientStandardRenderPasses.Add(renderPass);
            }

            return renderPass;
        }

        private void RenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass renderPass, IEntity[] entities, ILoadingScreen loadingScreen)
        {
            if (!renderPass.SkipWorldRenderBelow && gameContext.World != null)
            {
                gameContext.World.RenderBelow(gameContext, renderContext);
            }

            if (gameContext.World == null && loadingScreen != null)
            {
                loadingScreen.Render(gameContext, renderContext);
            }

            if (!renderPass.SkipEntityRender && entities != null)
            {
                foreach (var entity in entities.OfType<IPrerenderableEntity>())
                {
                    entity.Prerender(gameContext, renderContext);
                }

                renderContext.PostPreRender(gameContext);

                foreach (var entity in entities)
                {
                    entity.Render(gameContext, renderContext);
                }
            }
            else
            {
                renderContext.PostPreRender(gameContext);
            }

            if (!renderPass.SkipWorldRenderAbove && gameContext.World != null)
            {
                gameContext.World.RenderAbove(gameContext, renderContext);
            }

            if (!renderPass.SkipEngineHookRender)
            {
                foreach (var hook in _engineHooks)
                {
                    hook.Render(gameContext, renderContext);
                }
            }
        }

        public IRenderPass GetCurrentRenderPass()
        {
            return _renderPass;
        }

        public bool IsFirstRenderPass()
        {
            return _isFirstRenderPass;
        }
    }
}

