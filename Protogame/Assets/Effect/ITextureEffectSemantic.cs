using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface ITextureEffectSemantic : IEffectSemantic
    {
        Texture2D Texture { get; set; }
    }
}