using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultStandardDirectionalLight : IStandardDirectionalLight
    {
        public DefaultStandardDirectionalLight(
            ILightRenderer<IStandardDirectionalLight> lightRenderer,
            Vector3 lightDirection,
            Color lightColor)
        {
            LightRenderer = lightRenderer;
            LightDirection = lightDirection;
            LightColor = lightColor;
        }

        public Vector3 LightDirection { get; set; }

        public Color LightColor { get; set; }

        public ILightRenderer LightRenderer { get; }
    }
}