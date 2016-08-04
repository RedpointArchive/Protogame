using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class TextureEffectSemantic : ITextureEffectSemantic
    {
        private IEffectParameterSet _parameterSet;

        private IEffectWritableParameter _textureParam;

        private IEffectWritableParameter _textureDimensionsParam;

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

        public bool ShouldAttachToParameterSet(IEffectParameterSet parameterSet)
        {
            return parameterSet["Texture"] != null;
        }

        public void AttachToParameterSet(IEffectParameterSet parameterSet)
        {
            if (_parameterSet != null)
            {
                throw new InvalidOperationException("This semantic is already attached.");
            }

            _parameterSet = parameterSet;
            CacheParameters();
        }

        public IEffectSemantic Clone(IEffectParameterSet parameterSet)
        {
            var clone = new TextureEffectSemantic();
            clone.AttachToParameterSet(parameterSet);
            if (_parameterSet != null)
            {
                clone.Texture = this.Texture;
            }
            return clone;
        }

        public void CacheParameters()
        {
            if (_textureParam == null)
            {
                _textureParam = _parameterSet["Texture"];
            }

            if (_hasTextureDimensions == null)
            {
                _textureDimensionsParam = _parameterSet["TextureDimensions"];
                _hasTextureDimensions = _textureDimensionsParam != null;
            }
        }

        public void OnApply(IRenderContext renderContext)
        {
        }
    }
}
