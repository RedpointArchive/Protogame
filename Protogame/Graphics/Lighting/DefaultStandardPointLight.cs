using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultStandardPointLight : IStandardPointLight
    {
        private readonly EffectAsset _pointLightEffect;

        public DefaultStandardPointLight(
            IAssetManagerProvider assetManagerProvider,
            Vector3 lightPosition,
            Color lightColor,
            float lightRadius,
            float lightIntensity)
        {
            LightPosition = lightPosition;
            LightColor = lightColor;
            LightRadius = lightRadius;
            LightIntensity = lightIntensity;

            _pointLightEffect = assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.PointLight");
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, ILightContext lightContext)
        {
            _pointLightEffect.Effect.Parameters["Color"]?.SetValue(lightContext.DeferredColorMap);
            _pointLightEffect.Effect.Parameters["Normal"]?.SetValue(lightContext.DeferredNormalMap);
            _pointLightEffect.Effect.Parameters["Depth"]?.SetValue(lightContext.DeferredDepthMap);
            _pointLightEffect.Effect.Parameters["LightPosition"]?.SetValue(LightPosition);
            _pointLightEffect.Effect.Parameters["LightColor"]?.SetValue(LightColor.ToVector3());
            _pointLightEffect.Effect.Parameters["LightRadius"]?.SetValue(LightRadius);
            _pointLightEffect.Effect.Parameters["LightIntensity"]?.SetValue(LightIntensity);

            // TODO
        }

        public Vector3 LightPosition { get; }
        public Color LightColor { get; }
        public float LightRadius { get; }
        public float LightIntensity { get; }
    }
}