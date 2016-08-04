using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class ScreenDimensionsEffectSemantic : IScreenDimensionsEffectSemantic
    {
        private IEffectParameterSet _parameterSet;

        private IEffectWritableParameter _screenDimensionsParam;

        private int _lastScreenX;

        private int _lastScreenY;

        public bool ShouldAttachToParameterSet(IEffectParameterSet parameterSet)
        {
            return parameterSet["ScreenDimensions"] != null;
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
            var clone = new ScreenDimensionsEffectSemantic();
            clone.AttachToParameterSet(parameterSet);
            return clone;
        }

        public void CacheParameters()
        {
            if (_screenDimensionsParam == null)
            {
                _screenDimensionsParam = _parameterSet["ScreenDimensions"];
            }
        }

        public void OnApply(IRenderContext renderContext)
        {
            var width = renderContext.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var height = renderContext.GraphicsDevice.PresentationParameters.BackBufferHeight;

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
