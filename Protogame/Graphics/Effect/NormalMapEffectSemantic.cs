using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class NormalMapEffectSemantic : INormalMapEffectSemantic
    {
        private IEffectParameterSet _parameterSet;

        private IEffectWritableParameter _heightMapParam;

        public Texture2D NormalMap
        {
            get { return _heightMapParam.GetValueTexture2D(); }
            set { _heightMapParam.SetValue(value); }
        }

        public bool ShouldAttachToParameterSet(IEffectParameterSet parameterSet)
        {
            return parameterSet["NormalMap"] != null;
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
            var clone = new NormalMapEffectSemantic();
            clone.AttachToParameterSet(parameterSet);
            if (_parameterSet != null)
            {
                clone.NormalMap = this.NormalMap;
            }
            return clone;
        }

        public void CacheParameters()
        {
            if (_heightMapParam == null)
            {
                _heightMapParam = _parameterSet["NormalMap"];
            }
        }

        public void OnApply(IRenderContext renderContext)
        {
        }
    }
}
