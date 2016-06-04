using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            _pointLightEffect.Effect.Parameters["Color"]?.SetValue(lightContext.DeferredColorMap);
            _pointLightEffect.Effect.Parameters["Normal"]?.SetValue(lightContext.DeferredNormalMap);
            _pointLightEffect.Effect.Parameters["Depth"]?.SetValue(lightContext.DeferredDepthMap);
            _pointLightEffect.Effect.Parameters["CameraPosition"]?.SetValue(renderContext.CameraPosition);
            _pointLightEffect.Effect.Parameters["LightPosition"]?.SetValue(LightPosition);
            _pointLightEffect.Effect.Parameters["LightColor"]?.SetValue(LightColor.ToVector3());
            _pointLightEffect.Effect.Parameters["LightRadius"]?.SetValue(LightRadius);
            _pointLightEffect.Effect.Parameters["LightIntensity"]?.SetValue(LightIntensity);
            _pointLightEffect.Effect.Parameters["LightInvertViewProjection"]?.SetValue(Matrix.Invert(renderContext.View * renderContext.Projection));
            _pointLightEffect.Effect.Parameters["World"]?.SetValue(renderContext.World);
            _pointLightEffect.Effect.Parameters["View"]?.SetValue(renderContext.View);
            _pointLightEffect.Effect.Parameters["Projection"]?.SetValue(renderContext.Projection);
            
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

            renderContext.PushEffect(_pointLightEffect.Effect);

            if (lightContext.LightRenderTarget != null)
            {
                renderContext.PushRenderTarget(lightContext.LightRenderTarget);
            }

            _pointLightSphereModel.Render(renderContext, Matrix.Identity);

            if (lightContext.LightRenderTarget != null)
            {
                renderContext.PopRenderTarget();
            }

            renderContext.PopEffect();

            renderContext.World = oldWorld;
            renderContext.GraphicsDevice.RasterizerState = oldRasterizerState;
        }

        public Vector3 LightPosition { get; set; }
        public Color LightColor { get; set; }
        public float LightRadius { get; set; }
        public float LightIntensity { get; set; }
    }
}