using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultStandardPointLight : IStandardPointLight
    {
        public DefaultStandardPointLight(
            ILightRenderer<IStandardPointLight> lightRenderer,
            Vector3 lightPosition,
            Color lightColor,
            float lightRadius,
            float lightIntensity)
        {
            LightRenderer = lightRenderer;
            LightPosition = lightPosition;
            LightColor = lightColor;
            LightRadius = lightRadius;
            LightIntensity = lightIntensity;
        }

        public Vector3 LightPosition { get; set; }
        public Color LightColor { get; set; }
        public float LightRadius { get; set; }
        public float LightIntensity { get; set; }
        public ILightRenderer LightRenderer { get; }
    }
}