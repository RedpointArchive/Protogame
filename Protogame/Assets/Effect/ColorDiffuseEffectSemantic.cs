using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class ColorDiffuseEffectSemantic : IColorDiffuseEffectSemantic
    {
        private EffectWithSemantics _effectWithSemantics;

        private EffectParameter _diffuseParam;

        public Color Diffuse
        {
            get { return new Color(_diffuseParam.GetValueVector4()); }
            set { _diffuseParam.SetValue(value.ToVector4()); }
        }

        public bool ShouldAttachToEffect(EffectWithSemantics effectWithSemantics)
        {
            return effectWithSemantics.Parameters["ColorDiffuse"] != null;
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
            var clone = new ColorDiffuseEffectSemantic();
            clone.AttachToEffect(effectWithSemantics);
            clone.Diffuse = this.Diffuse;
            return clone;
        }

        public void CacheEffectParameters()
        {
            if (_diffuseParam == null)
            {
                _diffuseParam = _effectWithSemantics.Parameters["ColorDiffuse"];
            }
        }

        public void OnApply()
        {
        }
    }
}
