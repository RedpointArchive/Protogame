using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The implementation of <see cref="I3DRenderPass"/> which uses deferred rendering.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.I3DRenderPass</interface_ref>
    public class Default3DDeferredRenderPass : I3DDeferredRenderPass
    {
        private readonly IHierarchy _hierarchy;
        private readonly IRenderTargetBackBufferUtilities _renderTargetBackBufferUtilities;
        private readonly IGraphicsBlit _graphicsBlit;
        private readonly IRenderBatcher _renderBatcher;
        private readonly IBackBufferDimensions _backBufferDimensions;
        private readonly IAssetReference<EffectAsset> _gbufferClearEffect;
        private readonly IAssetReference<EffectAsset> _gbufferCombineEffect;

        private RenderTarget2D _normalRenderTarget;
        private RenderTarget2D _colorRenderTarget;
        private RenderTarget2D _depthRenderTarget;
        private RenderTarget2D _specularRenderTarget;
        private RenderTarget2D _diffuseLightRenderTarget;
        private RenderTarget2D _specularLightRenderTarget;

        private DepthStencilState _depthStencilState;
        private DepthStencilState _lightDepthStencilState;
        private RasterizerState _rasterizerStateCullNone;
        private RasterizerState _rasterizerStateCullClockwiseFace;
        private RasterizerState _rasterizerStateCullCounterClockwiseFace;
        private BlendState _blendState;
        private BlendState _lightBlendState;

        private DepthStencilState _previousDepthStencilState;
        private RasterizerState _previousRasterizerState;
        private BlendState _previousBlendState;

        public Default3DDeferredRenderPass(
            IHierarchy hierarchy,
            IRenderTargetBackBufferUtilities renderTargetBackBufferUtilities,
            IGraphicsBlit graphicsBlit,
            IAssetManager assetManager,
            IRenderBatcher renderBatcher,
            IBackBufferDimensions backBufferDimensions)
        {
            _hierarchy = hierarchy;
            _renderTargetBackBufferUtilities = renderTargetBackBufferUtilities;
            _graphicsBlit = graphicsBlit;
            _renderBatcher = renderBatcher;
            _backBufferDimensions = backBufferDimensions;
            _gbufferClearEffect =
                assetManager.Get<EffectAsset>("effect.GBufferClear");
            _gbufferCombineEffect =
                assetManager.Get<EffectAsset>("effect.GBufferCombine");

            GBufferBlendState = BlendState.Opaque;
            ClearTarget = true;
        }
        
        public bool IsPostProcessingPass => false;
        public bool SkipWorldRenderBelow => false;
        public bool SkipWorldRenderAbove => false;
        public bool SkipEntityRender => false;
        public bool SkipEngineHookRender => false;
        public string EffectTechniqueName => RenderPipelineTechniqueName.Deferred;

        public Viewport? Viewport { get; set; }

        public bool DebugGBuffer { get; set; }

        /// <summary>
        /// Clear the depth buffer before this render pass starts rendering.  This allows you to alpha blend
        /// a 3D deferred render pass on top of a 2D render pass, without the 2D render pass interfering
        /// with the rendering of 3D objects.
        /// </summary>
        public bool ClearDepthBuffer { get; set; }

        /// <summary>
        /// Clear the target before this render pass starts rendering.  If your scene doesn't fully cover
        /// the scene this should be turned on (unless you want what was previously rendered to remain on
        /// screen).  This is on by default.
        /// </summary>
        public bool ClearTarget { get; set; }

        /// <summary>
        /// The blend state to use when rendering the final G-buffer onto the backbuffer (or current
        /// render target).  By default this is opaque, which is probably what you want if the deferred
        /// rendering pass is the first in the pipeline.  However if you're rendering 2D content underneath
        /// the 3D content, you should set this to something like <see cref="BlendState.AlphaBlend"/>.
        /// </summary>
        public BlendState GBufferBlendState { get; set; }

        public void BeginRenderPass(
            IGameContext gameContext,
            IRenderContext renderContext, 
            IRenderPass previousPass,      
            RenderTarget2D postProcessingSource)
        {
            if (!_gbufferClearEffect.IsReady || !_gbufferCombineEffect.IsReady)
            {
                return;
            }

            _colorRenderTarget = _renderTargetBackBufferUtilities.UpdateCustomRenderTarget(_colorRenderTarget, gameContext, SurfaceFormat.Color, DepthFormat.Depth24, null);
            _normalRenderTarget = _renderTargetBackBufferUtilities.UpdateCustomRenderTarget(_normalRenderTarget, gameContext, SurfaceFormat.Color, DepthFormat.None, null);
            _depthRenderTarget = _renderTargetBackBufferUtilities.UpdateCustomRenderTarget(_depthRenderTarget, gameContext, SurfaceFormat.Single, DepthFormat.None, null);
            _specularRenderTarget = _renderTargetBackBufferUtilities.UpdateCustomRenderTarget(_specularRenderTarget, gameContext, SurfaceFormat.Color, DepthFormat.None, null);
            _diffuseLightRenderTarget = _renderTargetBackBufferUtilities.UpdateCustomRenderTarget(_diffuseLightRenderTarget, gameContext, null, DepthFormat.None, null);
            _specularLightRenderTarget = _renderTargetBackBufferUtilities.UpdateCustomRenderTarget(_specularLightRenderTarget, gameContext, null, DepthFormat.None, null);

            if (_rasterizerStateCullNone == null)
            {
                _rasterizerStateCullNone = new RasterizerState();
                _rasterizerStateCullNone.CullMode = CullMode.None;
                _rasterizerStateCullNone.FillMode = FillMode.Solid;
                _rasterizerStateCullNone.DepthBias = 0;
                _rasterizerStateCullNone.MultiSampleAntiAlias = true;
                _rasterizerStateCullNone.ScissorTestEnable = false;
                _rasterizerStateCullNone.SlopeScaleDepthBias = 0;
                _rasterizerStateCullNone.DepthClipEnable = true;
            }

            if (_rasterizerStateCullClockwiseFace == null)
            {
                _rasterizerStateCullClockwiseFace = new RasterizerState();
                _rasterizerStateCullClockwiseFace.CullMode = CullMode.CullClockwiseFace;
                _rasterizerStateCullClockwiseFace.FillMode = FillMode.Solid;
                _rasterizerStateCullClockwiseFace.DepthBias = 0;
                _rasterizerStateCullClockwiseFace.MultiSampleAntiAlias = true;
                _rasterizerStateCullClockwiseFace.ScissorTestEnable = false;
                _rasterizerStateCullClockwiseFace.SlopeScaleDepthBias = 0;
                _rasterizerStateCullClockwiseFace.DepthClipEnable = true;
            }

            if (_rasterizerStateCullCounterClockwiseFace == null)
            {
                _rasterizerStateCullCounterClockwiseFace = new RasterizerState();
                _rasterizerStateCullCounterClockwiseFace.CullMode = CullMode.CullCounterClockwiseFace;
                _rasterizerStateCullCounterClockwiseFace.FillMode = FillMode.Solid;
                _rasterizerStateCullCounterClockwiseFace.DepthBias = 0;
                _rasterizerStateCullCounterClockwiseFace.MultiSampleAntiAlias = true;
                _rasterizerStateCullCounterClockwiseFace.ScissorTestEnable = false;
                _rasterizerStateCullCounterClockwiseFace.SlopeScaleDepthBias = 0;
                _rasterizerStateCullCounterClockwiseFace.DepthClipEnable = true;
            }

            if (_depthStencilState == null)
            {
                _depthStencilState = DepthStencilState.Default;
            }

            if (_lightDepthStencilState == null)
            {
                _lightDepthStencilState = DepthStencilState.None;
            }

            if (_blendState == null)
            {
                _blendState = BlendState.Opaque;
            }

            if (_lightBlendState == null)
            {
                _lightBlendState = BlendState.AlphaBlend;
            }

            renderContext.PushRenderTarget(
                _colorRenderTarget,
                _normalRenderTarget,
                _depthRenderTarget,
                _specularRenderTarget);

            if (ClearDepthBuffer || ClearTarget)
            {
                var target = ClearDepthBuffer ? ClearOptions.DepthBuffer : ClearOptions.Target;
                if (ClearDepthBuffer && ClearTarget)
                {
                    target = ClearOptions.DepthBuffer | ClearOptions.Target;
                }
                renderContext.GraphicsDevice.Clear(
                    target,
                    Microsoft.Xna.Framework.Color.Transparent,
                    renderContext.GraphicsDevice.Viewport.MaxDepth,
                    0);
            }

            // Clear the geometry buffer before moving into main rendering.
            _graphicsBlit.Blit(
                renderContext,
                null,
                null,
                _gbufferClearEffect.Asset.Effect);

            _previousDepthStencilState = renderContext.GraphicsDevice.DepthStencilState;
            _previousRasterizerState = renderContext.GraphicsDevice.RasterizerState;
            _previousBlendState = renderContext.GraphicsDevice.BlendState;

            renderContext.GraphicsDevice.DepthStencilState = _depthStencilState;
            renderContext.GraphicsDevice.RasterizerState = _rasterizerStateCullCounterClockwiseFace;
            renderContext.GraphicsDevice.BlendState = _blendState;
        }

        public void EndRenderPass(
            IGameContext gameContext, 
            IRenderContext renderContext, 
            IRenderPass nextPass)
        {
            if (!_gbufferClearEffect.IsReady || !_gbufferCombineEffect.IsReady)
            {
                return;
            }

            _renderBatcher.FlushRequests(gameContext, renderContext);
            
            renderContext.PopRenderTarget();

            renderContext.GraphicsDevice.DepthStencilState = _previousDepthStencilState;
            renderContext.GraphicsDevice.RasterizerState = _previousRasterizerState;
            renderContext.GraphicsDevice.BlendState = _previousBlendState;
            
            renderContext.PushRenderTarget(_diffuseLightRenderTarget);
            renderContext.GraphicsDevice.Clear(Color.Transparent);
            renderContext.PopRenderTarget();

            renderContext.PushRenderTarget(_specularLightRenderTarget);
            renderContext.GraphicsDevice.Clear(Color.Transparent);
            renderContext.PopRenderTarget();

            var size = _backBufferDimensions.GetSize(renderContext.GraphicsDevice);
            var lightContext = new DefaultLightContext(
                _colorRenderTarget,
                _normalRenderTarget,
                _depthRenderTarget,
                _specularRenderTarget,
                _diffuseLightRenderTarget,
                _specularLightRenderTarget,
                _lightBlendState,
                _rasterizerStateCullNone,
                _rasterizerStateCullClockwiseFace,
                _rasterizerStateCullCounterClockwiseFace,
                new Vector2(
                    0.5f/size.X,
                    0.5f/size.Y));

            var lights = new List<ILight>();

            if (gameContext.World != null)
            { 
                foreach (var l in _hierarchy.Lookup(gameContext.World).Children.Select(x => x.UntypedValue).OfType<IHasLights>())
                {
                    lights.AddRange(l.GetLights());
                }
            }

            renderContext.GraphicsDevice.BlendState = _lightBlendState;
            renderContext.GraphicsDevice.DepthStencilState = _lightDepthStencilState;
            
            foreach (var renderPair in lights.GroupBy(x => x.LightRenderer))
            {
                renderPair.Key.Render(gameContext, renderContext, lightContext, renderPair);
            }

            renderContext.GraphicsDevice.BlendState = _previousBlendState;
            renderContext.GraphicsDevice.DepthStencilState = _previousDepthStencilState;

            if (DebugGBuffer)
            {
                // Render the G buffer into 4 quadrants on the current render target.
                renderContext.GraphicsDevice.Clear(Color.Purple);
                _graphicsBlit.Blit(
                    renderContext,
                    _colorRenderTarget,
                    null,
                    null,
                    null,
                    null,
                    new Vector2(0, 0),
                    new Vector2(0.33f, 0.5f));
                _graphicsBlit.Blit(
                    renderContext,
                    _normalRenderTarget,
                    null,
                    null,
                    null,
                    null,
                    new Vector2(0.33f, 0),
                    new Vector2(0.34f, 0.5f));
                _graphicsBlit.Blit(
                    renderContext,
                    _depthRenderTarget,
                    null,
                    null,
                    null,
                    null,
                    new Vector2(0.67f, 0f),
                    new Vector2(0.33f, 0.5f));
                _graphicsBlit.Blit(
                    renderContext,
                    _specularRenderTarget,
                    null,
                    null,
                    null,
                    null,
                    new Vector2(0.0f, 0.5f),
                    new Vector2(0.33f, 0.5f));
                _graphicsBlit.Blit(
                    renderContext,
                    _diffuseLightRenderTarget,
                    null,
                    null,
                    null,
                    null,
                    new Vector2(0.33f, 0.5f),
                    new Vector2(0.34f, 0.5f));
                _graphicsBlit.Blit(
                    renderContext,
                    _specularLightRenderTarget,
                    null,
                    null,
                    null,
                    null,
                    new Vector2(0.67f, 0.5f),
                    new Vector2(0.33f, 0.5f));
            }
            else
            {
                var parameterSet = _gbufferCombineEffect.Asset.Effect.CreateParameterSet();
                parameterSet["Color"]?.SetValue(_colorRenderTarget);
                parameterSet["DiffuseLight"]?.SetValue(_diffuseLightRenderTarget);
                parameterSet["SpecularLight"]?.SetValue(_specularLightRenderTarget);
                parameterSet["AmbientLight"]?.SetValue(new Vector3(0.2f, 0.2f, 0.2f));

                _graphicsBlit.Blit(
                    renderContext,
                    null,
                    null,
                    _gbufferCombineEffect.Asset.Effect,
                    parameterSet,
                    GBufferBlendState);
            }
        }

        public string Name { get; set; }
    }
}
