using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BonesEffectSemantic : IBonesEffectSemantic
    {
        private const int MAX_BONES = 48;

        private IEffectParameterSet _parameterSet;

        private Matrix[] _bones = new Matrix[MAX_BONES];

        private IEffectWritableParameter _bonesParam;

        public bool ShouldAttachToParameterSet(IEffectParameterSet parameterSet)
        {
            return parameterSet["Bones"] != null;
        }

        public void AttachToParameterSet(IEffectParameterSet parameterSet)
        {
            if (_parameterSet != null)
            {
                throw new InvalidOperationException("This semantic is already attached.");
            }

            _parameterSet = parameterSet;
            _bonesParam = parameterSet["Bones"];
        }

        public Matrix[] Bones
        {
            get { return _bones; }
        }

        public IEffectSemantic Clone(IEffectParameterSet parameterSet)
        {
            var clone = new BonesEffectSemantic();
            clone.AttachToParameterSet(parameterSet);
            if (_parameterSet != null)
            {
                clone._bones = this.Bones;
            }
            return clone;
        }

        public void CacheParameters()
        {
        }

        public void OnApply(IRenderContext renderContext)
        {
            _bonesParam.SetValue(_bones);
        }
    }
}
