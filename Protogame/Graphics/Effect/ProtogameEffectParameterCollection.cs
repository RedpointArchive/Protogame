using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class ProtogameEffectParameterCollection : IEffectParameterCollection
    {
        private readonly Effect _targetEffect;

        public ProtogameEffectParameterCollection(Effect targetEffect)
        {
            _targetEffect = targetEffect;
        }

        IEffectParameter IEffectParameterCollection.this[int index]
        {
            get
            {
                if (_targetEffect.Parameters[index] != null)
                {
                    return new ProtogameEffectParameter(_targetEffect, index);
                }

                return null;
            }
        }

        IEffectParameter IEffectParameterCollection.this[string name]
        {
            get
            {
                if (_targetEffect.Parameters[name] != null)
                {
                    return new ProtogameEffectParameter(_targetEffect, name);
                }

                return null;
            }
        }
    }
}