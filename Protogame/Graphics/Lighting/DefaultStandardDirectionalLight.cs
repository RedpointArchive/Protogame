using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultStandardDirectionalLight : IStandardDirectionalLight
    {
        private readonly IGraphicsBlit _graphicsBlit;
        private readonly IAssetReference<EffectAsset> _directionalLightEffect;

        public DefaultStandardDirectionalLight(
            IAssetManager assetManager,
            IGraphicsBlit graphicsBlit,
            Vector3 lightDirection,
            Color lightColor)
        {
            _graphicsBlit = graphicsBlit;
            LightDirection = lightDirection;
            LightColor = lightColor;

            _directionalLightEffect = assetManager.Get<EffectAsset>("effect.DirectionalLight");
        }

        public Vector3 LightDirection { get; set; }

        public Color LightColor { get; set; }

        public void Render(IGameContext gameContext, IRenderContext renderContext, ILightContext lightContext)
        {
            if (!_directionalLightEffect.IsReady)
            {
                return;
            }

            var parameterSet = _directionalLightEffect.Asset.Effect.CreateParameterSet();
            parameterSet["Color"]?.SetValue(lightContext.DeferredColorMap);
            parameterSet["Normal"]?.SetValue(lightContext.DeferredNormalMap);
            parameterSet["Depth"]?.SetValue(lightContext.DeferredDepthMap);
            parameterSet["LightDirection"]?.SetValue(LightDirection);
            parameterSet["LightColor"]?.SetValue(LightColor.ToVector3());
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