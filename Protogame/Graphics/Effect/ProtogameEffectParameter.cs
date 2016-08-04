using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class ProtogameEffectParameter : IEffectParameter
    {
        private readonly EffectParameter _targetParameter;

        public ProtogameEffectParameter(Effect targetEffect, int index)
        {
            _targetParameter = targetEffect.Parameters[index];
        }

        public ProtogameEffectParameter(Effect targetEffect, string name)
        {
            _targetParameter = targetEffect.Parameters[name];
        }

        public string Name => _targetParameter.Name;

        public Texture2D GetValueTexture2D()
        {
            return _targetParameter.GetValueTexture2D();
        }

        public Vector4 GetValueVector4()
        {
            return _targetParameter.GetValueVector4();
        }
    }
}