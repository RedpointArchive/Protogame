using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultStandardDirectionalLight : IStandardDirectionalLight
    {
        private readonly IGraphicsBlit _graphicsBlit;
        private readonly EffectAsset _directionalLightEffect;

        public DefaultStandardDirectionalLight(
            IAssetManagerProvider assetManagerProvider,
            IGraphicsBlit graphicsBlit,
            Vector3 lightDirection,
            Color lightColor)
        {
            _graphicsBlit = graphicsBlit;
            LightDirection = lightDirection;
            LightColor = lightColor;

            _directionalLightEffect = assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.DirectionalLight");
        }

        public Vector3 LightDirection { get; set; }

        public Color LightColor { get; set; }

        public void Render(IGameContext gameContext, IRenderContext renderContext, ILightContext lightContext)
        {
            var parameterSet = _directionalLightEffect.Effect.CreateParameterSet();
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
                _directionalLightEffect.Effect,
                parameterSet,
                lightContext.LightBlendState);
        }
    }
}