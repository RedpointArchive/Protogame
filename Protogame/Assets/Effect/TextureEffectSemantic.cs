using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class TextureEffectSemantic : ITextureEffectSemantic
    {
        private EffectWithSemantics _effectWithSemantics;

        private EffectParameter _textureParam;

        public Texture2D Texture
        {
            get { return _textureParam.GetValueTexture2D(); }
            set { _textureParam.SetValue(value); }
        }

        public bool ShouldAttachToEffect(EffectWithSemantics effectWithSemantics)
        {
            return effectWithSemantics.Parameters["Texture"] != null;
        }

        public void AttachToEffect(EffectWithSemantics effectWithSemantics)
        {
            if (_effectWithSemantics != null)
            {
                throw new InvalidOperationException("This semantic is already attached.");
            }

            _effectWithSemantics = effectWithSemantics;
            CacheEffectParameters();
        }

        public IEffectSemantic Clone(EffectWithSemantics effectWithSemantics)
        {
            var clone = new TextureEffectSemantic();
            clone.AttachToEffect(effectWithSemantics);
            clone.Texture = this.Texture;
            return clone;
        }

        public void CacheEffectParameters()
        {
            if (_textureParam == null)
            {
                _textureParam = _effectWithSemantics.Parameters["Texture"];
            }
        }

        public void OnApply()
        {
        }
    }
}
