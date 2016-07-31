using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class TextureEffectSemantic : ITextureEffectSemantic
    {
        private EffectWithSemantics _effectWithSemantics;

        private EffectParameter _textureParam;

        private EffectParameter _textureDimensionsParam;

        private bool? _hasTextureDimensions;

        public Texture2D Texture
        {
            get { return _textureParam.GetValueTexture2D(); }
            set
            {
                _textureParam.SetValue(value);

                if (_hasTextureDimensions ?? false)
                {
                    _textureDimensionsParam.SetValue(new Vector2(value.Width, value.Height));
                }
            }
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

            if (_hasTextureDimensions == null)
            {
                _textureDimensionsParam = _effectWithSemantics.Parameters["TextureDimensions"];
                _hasTextureDimensions = _textureDimensionsParam != null;
            }
        }

        public void OnApply()
        {
        }
    }
}
