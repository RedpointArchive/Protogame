using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface ISpecularEffectSemantic : IEffectSemantic
    {
        Texture2D SpecularColorMap { get; set; }

        Color SpecularColor { get; set; }

        float SpecularPower { get; set; }
    }
}