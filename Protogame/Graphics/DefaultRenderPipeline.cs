//#define DISABLE_PIPELINE_TARGETS

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="IRenderPipeline"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IRenderPipeline</interface_ref>
    public class DefaultRenderPipeline : IRenderPipeline
    {
        private readonly List<IRenderPass> _standardRenderPasses;

        private readonly List<IRenderPass> _postProcessingRenderPasses;

        private readonly List<IRenderPass> _transientStandardRenderPasses;

        private readonly List<IRenderPass> _transientPostProcessingRenderPasses;

        private readonly IGraphicsBlit _graphicsBlit;

        private readonly IRenderTargetBackBufferUtilities _renderTargetBackBufferUtilities;

        private RenderTarget2D _primary;

        private RenderTarget2D _secondary;

        public DefaultRenderPipeline(
            IGraphicsBlit graphicsBlit,
            IRenderTargetBackBufferUtilities renderTargetBackBufferUtilities)
        {
            _graphicsBlit = graphicsBlit;
            _renderTargetBackBufferUtilities = renderTargetBackBufferUtilities;
            _standardRenderPasses = new List<IRenderPass>();
            _postProcessingRenderPasses = new List<IRenderPass>();
            _transientStandardRenderPasses = new List<IRenderPass>();
            _transientPostProcessingRenderPasses = new List<IRenderPass>();
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            renderContext.Render(gameContext);

            _primary = _renderTargetBackBufferUtilities.UpdateRenderTarget(_primary, gameContext);
            _secondary = _renderTargetBackBufferUtilities.UpdateRenderTarget(_secondary, gameContext);

            var standardRenderPasses = _standardRenderPasses.ToArray();
            var postProcessingRenderPasses = _postProcessingRenderPasses.ToArray();
            //IRenderPass[] transientStandardRenderPasses;
            //IRenderPass[] transientPostProcessingRenderPasses;
            IRenderPass previousPass = null;
            IRenderPass nextPass = null;

            var entities = gameContext.World.Entities.ToArray();

#if !DISABLE_PIPELINE_TARGETS
            renderContext.PushRenderTarget(_primary);
#endif

            for (var i = 0; i < standardRenderPasses.Length; i++)
            {
                var pass = standardRenderPasses[i];
                ((RenderPipelineRenderContext)renderContext).CurrentRenderPass = pass;
                pass.BeginRenderPass(gameContext, renderContext, previousPass, null);
                previousPass = pass;
                RenderPass(gameContext, renderContext, entities);
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

            var loop = 100;
            while (_transientStandardRenderPasses.Count > 0 && loop-- >= 0)
            {
                var transientStandardRenderPasses = _transientStandardRenderPasses.ToArray();
                _transientStandardRenderPasses.Clear();

                for (var i = 0; i < transientStandardRenderPasses.Length; i++)
                {
                    var pass = transientStandardRenderPasses[i];
                    ((RenderPipelineRenderContext)renderContext).CurrentRenderPass = pass;
                    pass.BeginRenderPass(gameContext, renderContext, previousPass, null);
                    previousPass = pass;
                    RenderPass(gameContext, renderContext, entities);
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
            if (loop < 0)
            {
                throw new InvalidOperationException(
                    "Exceeded the number of AppendRenderPass iterations (100).  Ensure you " +
                    "are not unconditionally calling AppendRenderPass within another render pass.");
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
                ((RenderPipelineRenderContext)renderContext).CurrentRenderPass = pass;
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

            loop = 100;
            while (_transientPostProcessingRenderPasses.Count > 0 && loop-- >= 0)
            {
                var transientPostProcessingRenderPasses = _transientPostProcessingRenderPasses.ToArray();
                _transientPostProcessingRenderPasses.Clear();

                for (var i = 0; i < transientPostProcessingRenderPasses.Length; i++)
                {
                    var pass = transientPostProcessingRenderPasses[i];
                    ((RenderPipelineRenderContext)renderContext).CurrentRenderPass = pass;
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
            if (loop < 0)
            {
                throw new InvalidOperationException(
                    "Exceeded the number of AppendRenderPass iterations (100).  Ensure you " +
                    "are not unconditionally calling AppendRenderPass within another render pass.");
            }

#if !DISABLE_PIPELINE_TARGETS
            renderContext.PopRenderTarget();

            _graphicsBlit.Blit(renderContext, currentSource);
#endif
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

        private void RenderPass(IGameContext gameContext, IRenderContext renderContext, IEntity[] entities)
        {
            gameContext.World.RenderBelow(gameContext, renderContext);

            foreach (var entity in entities)
            {
                entity.Render(gameContext, renderContext);
            }

            gameContext.World.RenderAbove(gameContext, renderContext);
        }
    }
}

