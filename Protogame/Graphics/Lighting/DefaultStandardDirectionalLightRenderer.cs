using System.Collections.Generic;

namespace Protogame
{
    public class DefaultStandardDirectionalLightRenderer : ILightRenderer<IStandardDirectionalLight>
    {
        private readonly IGraphicsBlit _graphicsBlit;
        private readonly IAssetReference<EffectAsset> _directionalLightEffect;

        public DefaultStandardDirectionalLightRenderer(
            IAssetManager assetManager,
            IGraphicsBlit graphicsBlit)
        {
            _graphicsBlit = graphicsBlit;

            _directionalLightEffect = assetManager.Get<EffectAsset>("effect.DirectionalLight");
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, ILightContext lightContext, IEnumerable<ILight> lights)
        {
            if (!_directionalLightEffect.IsReady)
            {
                return;
            }

            foreach (var ulight in lights)
            {
                var light = (IStandardDirectionalLight)ulight;

                var parameterSet = _directionalLightEffect.Asset.Effect.CreateParameterSet();
                parameterSet["Color"]?.SetValue(lightContext.DeferredColorMap);
                parameterSet["Normal"]?.SetValue(lightContext.DeferredNormalMap);
                parameterSet["Depth"]?.SetValue(lightContext.DeferredDepthMap);
                parameterSet["LightDirection"]?.SetValue(light.LightDirection);
                parameterSet["LightColor"]?.SetValue(light.LightColor.ToVector3());
                parameterSet["HalfPixel"]?.SetValue(lightContext.HalfPixel);

                _graphicsBlit.BlitMRT(
                    renderContext,
                    null,
                    new[] { lightContext.DiffuseLightRenderTarget, lightContext.SpecularLightRenderTarget },
                    _directionalLightEffect.Asset.Effect,
                    parameterSet,
                    lightContext.LightBlendState);
            }
        }
    }
}
