using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class ScreenDimensionsEffectSemantic : IScreenDimensionsEffectSemantic
    {
        private EffectWithSemantics _effectWithSemantics;

        private EffectParameter _screenDimensionsParam;

        private int _lastScreenX;

        private int _lastScreenY;

        public bool ShouldAttachToEffect(EffectWithSemantics effectWithSemantics)
        {
            return effectWithSemantics.Parameters["ScreenDimensions"] != null;
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
            var clone = new ScreenDimensionsEffectSemantic();
            clone.AttachToEffect(effectWithSemantics);
            return clone;
        }

        public void CacheEffectParameters()
        {
            if (_screenDimensionsParam == null)
            {
                _screenDimensionsParam = _effectWithSemantics.Parameters["ScreenDimensions"];
            }
        }

        public void OnApply()
        {
            var width = _effectWithSemantics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var height = _effectWithSemantics.GraphicsDevice.PresentationParameters.BackBufferHeight;

            var needsUpdate = false;
            if (_lastScreenX != width)
            {
                _lastScreenX = width;
                needsUpdate = true;
            }
            if (_lastScreenY != height)
            {
                _lastScreenY = height;
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                _screenDimensionsParam.SetValue(new Vector2(_lastScreenX, _lastScreenY));
            }
        }
    }
}
