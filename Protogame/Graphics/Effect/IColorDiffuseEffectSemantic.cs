using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IColorDiffuseEffectSemantic : IEffectSemantic
    {
        Color Diffuse { get; set; }
    }
}