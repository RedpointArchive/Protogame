using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class BonesEffectSemantic : IBonesEffectSemantic
    {
        private const int MAX_BONES = 48;

        private EffectWithSemantics _effectWithSemantics;

        private EffectParameter _bonesParam;

        private Matrix[] _bones = new Matrix[MAX_BONES];

        public bool ShouldAttachToEffect(EffectWithSemantics effectWithSemantics)
        {
            return effectWithSemantics.Parameters["Bones"] != null;
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

        public Matrix[] Bones
        {
            get { return _bones; }
        }

        public IEffectSemantic Clone(EffectWithSemantics effectWithSemantics)
        {
            var clone = new BonesEffectSemantic();
            clone.AttachToEffect(effectWithSemantics);
            clone._bones = this.Bones;
            return clone;
        }

        public void CacheEffectParameters()
        {
            if (_bonesParam == null)
            {
                _bonesParam = _effectWithSemantics.Parameters["Bones"];
            }
        }

        public void OnApply()
        {
            _bonesParam.SetValue(_bones);
        }
    }
}
