using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface ISpecularEffectSemantic : IEffectSemantic
    {
        Texture2D SpecularIntensityMap { get; set; }

        Texture2D SpecularColorMap { get; set; }

        float SpecularIntensity { get; set; }

        Color SpecularColor { get; set; }

        float SpecularPower { get; set; }
    }
}