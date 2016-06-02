using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IStandardPointLight : ILight
    {
        Vector3 LightPosition { get; }

        Color LightColor { get; }

        float LightRadius { get; }

        float LightIntensity { get; }
    }
}