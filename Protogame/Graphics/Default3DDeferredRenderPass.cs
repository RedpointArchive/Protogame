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
        private readonly ILightFactory _lightFactory;
        private readonly EffectAsset _gbufferClearEffect;
        private readonly EffectAsset _gbufferRenderEffect;
        private readonly EffectAsset _gbufferCombineEffect;

        private RenderTarget2D _normalRenderTarget;
        private RenderTarget2D _colorRenderTarget;
        private RenderTarget2D _depthRenderTarget;
        private RenderTarget2D _lightRenderTarget;

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
            IAssetManagerProvider assetManagerProvider,
            ILightFactory lightFactory)
        {
            _hierarchy = hierarchy;
            _renderTargetBackBufferUtilities = renderTargetBackBufferUtilities;
            _graphicsBlit = graphicsBlit;
            _lightFactory = lightFactory;
            _gbufferClearEffect =
                assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.GBufferClear");
            _gbufferRenderEffect =
                assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.GBufferRender");
            _gbufferCombineEffect =
                assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.GBufferCombine");
        }

        public bool IsPostProcessingPass => false;

        public bool DebugGBuffer { get; set; }

        public string EffectTechniqueName
        {
            get { return RenderPipelineTechniqueName.Deferred; }
        }

        public void BeginRenderPass(
            IGameContext gameContext,
            IRenderContext renderContext, 
            IRenderPass previousPass,      
            RenderTarget2D postProcessingSource)
        {
            _colorRenderTarget = _renderTargetBackBufferUtilities.UpdateCustomRenderTarget(_colorRenderTarget, gameContext, SurfaceFormat.Color, DepthFormat.Depth24);
            _normalRenderTarget = _renderTargetBackBufferUtilities.UpdateCustomRenderTarget(_normalRenderTarget, gameContext, SurfaceFormat.Color, DepthFormat.None);
            _depthRenderTarget = _renderTargetBackBufferUtilities.UpdateCustomRenderTarget(_depthRenderTarget, gameContext, SurfaceFormat.Single, DepthFormat.None);
            _lightRenderTarget = _renderTargetBackBufferUtilities.UpdateCustomRenderTarget(_lightRenderTarget, gameContext, null, DepthFormat.None);

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
                _depthRenderTarget);

            // Clear the geometry buffer before moving into main rendering.
            _graphicsBlit.Blit(
                renderContext,
                null,
                null,
                _gbufferClearEffect.Effect);

            renderContext.PushEffect(
                _gbufferRenderEffect.Effect);

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
            renderContext.PopEffect();
            renderContext.PopRenderTarget();

            renderContext.GraphicsDevice.DepthStencilState = _previousDepthStencilState;
            renderContext.GraphicsDevice.RasterizerState = _previousRasterizerState;
            renderContext.GraphicsDevice.BlendState = _previousBlendState;
            
            renderContext.PushRenderTarget(_lightRenderTarget);
            renderContext.GraphicsDevice.Clear(Color.Transparent);
            renderContext.PopRenderTarget();

            var lightContext = new DefaultLightContext(
                _colorRenderTarget,
                _normalRenderTarget,
                _depthRenderTarget,
                _lightRenderTarget,
                _lightBlendState,
                _rasterizerStateCullNone,
                _rasterizerStateCullClockwiseFace,
                _rasterizerStateCullCounterClockwiseFace,
                new Vector2(
                    0.5f/renderContext.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    0.5f/renderContext.GraphicsDevice.PresentationParameters.BackBufferHeight));

            var lights = new List<ILight>();

            foreach (var l in _hierarchy.Lookup(gameContext.World).Children.Select(x => x.UntypedValue).OfType<IHasLights>())
            {
                lights.AddRange(l.GetLights());
            }

            renderContext.GraphicsDevice.BlendState = _lightBlendState;
            renderContext.GraphicsDevice.DepthStencilState = _lightDepthStencilState;

            foreach (var light in lights)
            {
                light.Render(gameContext, renderContext, lightContext);
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
                    new Vector2(0, 0),
                    new Vector2(0.5f, 0.5f));
                _graphicsBlit.Blit(
                    renderContext,
                    _normalRenderTarget,
                    null,
                    null,
                    null,
                    new Vector2(0.5f, 0),
                    new Vector2(0.5f, 0.5f));
                _graphicsBlit.Blit(
                    renderContext,
                    _depthRenderTarget,
                    null,
                    null,
                    null,
                    new Vector2(0f, 0.5f),
                    new Vector2(0.5f, 0.5f));
                _graphicsBlit.Blit(
                    renderContext,
                    _lightRenderTarget,
                    null,
                    null,
                    BlendState.AlphaBlend,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f));
            }
            else
            { 
                _gbufferCombineEffect.Effect.Parameters["Color"]?.SetValue(_colorRenderTarget);
                _gbufferCombineEffect.Effect.Parameters["Light"]?.SetValue(_lightRenderTarget);
                _gbufferCombineEffect.Effect.Parameters["AmbientLight"]?.SetValue(new Vector3(0.2f, 0.2f, 0.2f));

                _graphicsBlit.Blit(
                    renderContext,
                    null,
                    null,
                    _gbufferCombineEffect.Effect,
                    BlendState.Opaque);
            }
        }
    }
}
