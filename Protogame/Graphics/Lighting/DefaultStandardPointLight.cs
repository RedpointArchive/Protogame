using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultStandardPointLight : IStandardPointLight
    {
        private readonly EffectAsset _pointLightEffect;

        private readonly ModelAsset _pointLightSphereModel;

        private readonly IEffectParameterSet _parameterSet;

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
            _pointLightSphereModel = assetManagerProvider.GetAssetManager().Get<ModelAsset>("model.LightSphere");

            _parameterSet = _pointLightEffect.Effect.CreateParameterSet();
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, ILightContext lightContext)
        {
            var oldRasterizerState = renderContext.GraphicsDevice.RasterizerState;
            var oldWorld = renderContext.World;

            _parameterSet.Unlock();
            _parameterSet["Color"]?.SetValue(lightContext.DeferredColorMap);
            _parameterSet["Normal"]?.SetValue(lightContext.DeferredNormalMap);
            _parameterSet["Depth"]?.SetValue(lightContext.DeferredDepthMap);
            _parameterSet["Specular"]?.SetValue(lightContext.DeferredSpecularMap);
            _parameterSet["CameraPosition"]?.SetValue(renderContext.CameraPosition);
            _parameterSet["LightPosition"]?.SetValue(LightPosition);
            _parameterSet["LightColor"]?.SetValue(LightColor.ToVector3());
            _parameterSet["LightRadius"]?.SetValue(LightRadius);
            _parameterSet["LightIntensity"]?.SetValue(LightIntensity);
            _parameterSet["LightInvertViewProjection"]?.SetValue(Matrix.Invert(renderContext.View * renderContext.Projection));
            _parameterSet["World"]?.SetValue(renderContext.World);
            _parameterSet["View"]?.SetValue(renderContext.View);
            _parameterSet["Projection"]?.SetValue(renderContext.Projection);
            
            // If we are inside the lighting sphere, we need to invert the culling mode so
            // that we see the inside faces of the model.
            float cameraToCenter = Vector3.Distance(renderContext.CameraPosition, LightPosition);
            if (cameraToCenter < LightRadius)
            {
                renderContext.GraphicsDevice.RasterizerState = lightContext.RasterizerStateCullClockwiseFace;
            }
            else
            {
                renderContext.GraphicsDevice.RasterizerState = lightContext.RasterizerStateCullCounterClockwiseFace;
            }

            renderContext.World = Matrix.CreateScale(LightRadius) * Matrix.CreateTranslation(LightPosition);
            
            if (lightContext.DiffuseLightRenderTarget != null && lightContext.SpecularLightRenderTarget != null)
            {
                renderContext.PushRenderTarget(lightContext.DiffuseLightRenderTarget, lightContext.SpecularLightRenderTarget);
            }

            _pointLightSphereModel.Render(renderContext, _pointLightEffect.Effect, _parameterSet, Matrix.Identity);

            if (lightContext.DiffuseLightRenderTarget != null && lightContext.SpecularLightRenderTarget != null)
            {
                renderContext.PopRenderTarget();
            }

            renderContext.World = oldWorld;
            renderContext.GraphicsDevice.RasterizerState = oldRasterizerState;
        }

        public Vector3 LightPosition { get; set; }
        public Color LightColor { get; set; }
        public float LightRadius { get; set; }
        public float LightIntensity { get; set; }
    }
}