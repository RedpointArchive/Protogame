using System;

namespace Protogame
{
    public class CameraPositionEffectSemantic : ICameraPositionEffectSemantic
    {
        private IEffectParameterSet _parameterSet;

        private IEffectWritableParameter _cameraPositionParam;
        
        public bool ShouldAttachToParameterSet(IEffectParameterSet parameterSet)
        {
            return parameterSet["CameraPosition"] != null;
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
            var clone = new CameraPositionEffectSemantic();
            clone.AttachToParameterSet(parameterSet);
            return clone;
        }

        public void CacheParameters()
        {
            if (_cameraPositionParam == null)
            {
                _cameraPositionParam = _parameterSet["CameraPosition"];
            }
        }

        public void OnApply(IRenderContext renderContext)
        {
            _cameraPositionParam.SetValue(renderContext.CameraPosition);
        }
    }
}
