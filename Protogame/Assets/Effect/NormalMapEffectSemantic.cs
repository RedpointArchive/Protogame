using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class NormalMapEffectSemantic : INormalMapEffectSemantic
    {
        private EffectWithSemantics _effectWithSemantics;

        private EffectParameter _heightMapParam;

        public Texture2D NormalMap
        {
            get { return _heightMapParam.GetValueTexture2D(); }
            set { _heightMapParam.SetValue(value); }
        }

        public bool ShouldAttachToEffect(EffectWithSemantics effectWithSemantics)
        {
            return effectWithSemantics.Parameters["NormalMap"] != null;
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
            var clone = new NormalMapEffectSemantic();
            clone.AttachToEffect(effectWithSemantics);
            clone.NormalMap = this.NormalMap;
            return clone;
        }

        public void CacheEffectParameters()
        {
            if (_heightMapParam == null)
            {
                _heightMapParam = _effectWithSemantics.Parameters["NormalMap"];
            }
        }

        public void OnApply()
        {
        }
    }
}
