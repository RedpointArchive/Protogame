using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IBonesEffectSemantic : IEffectSemantic
    {
        Matrix[] Bones { get; }
    }
}