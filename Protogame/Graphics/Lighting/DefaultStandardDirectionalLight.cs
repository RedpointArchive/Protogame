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
            _directionalLightEffect.Effect.Parameters["Color"]?.SetValue(lightContext.DeferredColorMap);
            _directionalLightEffect.Effect.Parameters["Normal"]?.SetValue(lightContext.DeferredNormalMap);
            _directionalLightEffect.Effect.Parameters["Depth"]?.SetValue(lightContext.DeferredDepthMap);
            _directionalLightEffect.Effect.Parameters["LightDirection"]?.SetValue(LightDirection);
            _directionalLightEffect.Effect.Parameters["LightColor"]?.SetValue(LightColor.ToVector3());
            _directionalLightEffect.Effect.Parameters["HalfPixel"]?.SetValue(lightContext.HalfPixel);

            _graphicsBlit.Blit(
                renderContext,
                null,
                lightContext.LightRenderTarget,
                _directionalLightEffect.Effect,
                lightContext.LightBlendState);
        }
    }
}