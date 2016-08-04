using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IEffectParameter
    {
        string Name { get; }

        Texture2D GetValueTexture2D();

        Vector4 GetValueVector4();
    }
}