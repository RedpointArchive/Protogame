using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class SpecularEffectSemantic : ISpecularEffectSemantic
    {
        private IEffectParameterSet _parameterSet;

        private IEffectWritableParameter _specularIntensityMapParam;

        private IEffectWritableParameter _specularColorMapParam;

        private IEffectWritableParameter _specularIntensityParam;

        private IEffectWritableParameter _specularColorParam;

        private IEffectWritableParameter _specularPowerParam;

        public Texture2D SpecularIntensityMap
        {
            get { return _specularIntensityMapParam.GetValueTexture2D(); }
            set { _specularIntensityMapParam.SetValue(value); }
        }

        public Texture2D SpecularColorMap
        {
            get { return _specularColorMapParam.GetValueTexture2D(); }
            set { _specularColorMapParam.SetValue(value); }
        }

        public float SpecularIntensity
        {
            get { return _specularIntensityParam.GetValueSingle(); }
            set { _specularIntensityParam.SetValue(value); }
        }

        public Color SpecularColor
        {
            get { return _specularColorParam.GetValueColor(); }
            set { _specularColorParam.SetValue(value); }
        }

        public float SpecularPower
        {
            get { return _specularPowerParam.GetValueSingle(); }
            set { _specularPowerParam.SetValue(value); }
        }

        public bool ShouldAttachToParameterSet(IEffectParameterSet parameterSet)
        {
            return parameterSet["SpecularPower"] != null;
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
            var clone = new SpecularEffectSemantic();
            clone.AttachToParameterSet(parameterSet);
            if (_parameterSet != null)
            {
                // TODO
            }
            return clone;
        }

        public void CacheParameters()
        {
            if (_specularPowerParam == null)
            {
                _specularPowerParam = _parameterSet["SpecularPower"];
            }

            if (_specularIntensityParam == null)
            {
                _specularIntensityParam = _parameterSet["SpecularIntensity"];
            }

            if (_specularColorParam == null)
            {
                _specularColorParam = _parameterSet["SpecularColor"];
            }

            if (_specularIntensityMapParam == null)
            {
                _specularIntensityMapParam = _parameterSet["SpecularIntensityMap"];
            }

            if (_specularColorMapParam == null)
            {
                _specularColorMapParam = _parameterSet["SpecularColorMap"];
            }
        }

        public void OnApply(IRenderContext renderContext)
        {
        }
    }
}
