using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultStandardPointLight : IStandardPointLight
    {
        private readonly EffectAsset _pointLightEffect;

        private readonly ModelAsset _pointLightSphereModel;

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
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, ILightContext lightContext)
        {
            var oldRasterizerState = renderContext.GraphicsDevice.RasterizerState;
            var oldWorld = renderContext.World;

            var parameterSet = _pointLightEffect.Effect.CreateParameterSet();
            parameterSet["Color"]?.SetValue(lightContext.DeferredColorMap);
            parameterSet["Normal"]?.SetValue(lightContext.DeferredNormalMap);
            parameterSet["Depth"]?.SetValue(lightContext.DeferredDepthMap);
            parameterSet["CameraPosition"]?.SetValue(renderContext.CameraPosition);
            parameterSet["LightPosition"]?.SetValue(LightPosition);
            parameterSet["LightColor"]?.SetValue(LightColor.ToVector3());
            parameterSet["LightRadius"]?.SetValue(LightRadius);
            parameterSet["LightIntensity"]?.SetValue(LightIntensity);
            parameterSet["LightInvertViewProjection"]?.SetValue(Matrix.Invert(renderContext.View * renderContext.Projection));
            parameterSet["World"]?.SetValue(renderContext.World);
            parameterSet["View"]?.SetValue(renderContext.View);
            parameterSet["Projection"]?.SetValue(renderContext.Projection);
            
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
            
            if (lightContext.LightRenderTarget != null)
            {
                renderContext.PushRenderTarget(lightContext.LightRenderTarget);
            }

            _pointLightSphereModel.Render(renderContext, _pointLightEffect.Effect, parameterSet, Matrix.Identity);

            if (lightContext.LightRenderTarget != null)
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