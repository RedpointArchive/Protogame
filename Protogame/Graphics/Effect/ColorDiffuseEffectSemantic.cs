using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class ColorDiffuseEffectSemantic : IColorDiffuseEffectSemantic
    {
        private IEffectParameterSet _parameterSet;

        private IEffectWritableParameter _diffuseParam;

        public Color Diffuse
        {
            get { return new Color(_diffuseParam.GetValueVector4()); }
            set { _diffuseParam.SetValue(value.ToVector4()); }
        }

        public bool ShouldAttachToParameterSet(IEffectParameterSet parameterSet)
        {
            return parameterSet["ColorDiffuse"] != null;
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
            var clone = new ColorDiffuseEffectSemantic();
            clone.AttachToParameterSet(parameterSet);
            if (_parameterSet != null)
            {
                clone.Diffuse = this.Diffuse;
            }
            return clone;
        }

        public void CacheParameters()
        {
            if (_diffuseParam == null)
            {
                _diffuseParam = _parameterSet["ColorDiffuse"];
            }
        }

        public void OnApply(IRenderContext renderContext)
        {
        }
    }
}
