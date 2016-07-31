using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface INormalMapEffectSemantic : IEffectSemantic
    {
        Texture2D NormalMap { get; set; }
    }
}