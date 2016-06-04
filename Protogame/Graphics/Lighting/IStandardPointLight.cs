using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IStandardPointLight : ILight
    {
        Vector3 LightPosition { get; set; }

        Color LightColor { get; set; }

        float LightRadius { get; set; }

        float LightIntensity { get; set; }
    }
}