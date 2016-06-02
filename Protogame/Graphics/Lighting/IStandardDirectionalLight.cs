using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IStandardDirectionalLight : ILight
    {
        Vector3 LightDirection { get; set; }

        Color LightColor { get; set; }
    }
}