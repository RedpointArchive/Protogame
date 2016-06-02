using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public interface ILightFactory : IGenerateFactory
    {
        IStandardDirectionalLight CreateStandardDirectionalLight(Vector3 lightDirection, Color lightColor);

        IStandardPointLight CreateStandardPointLight(
            Vector3 lightPosition,
            Color lightColor,
            float lightRadius,
            float lightIntensity);
    }
}
