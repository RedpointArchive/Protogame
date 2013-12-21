using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class EffectWithTexture : Effect, IEffectTexture
    {
        private EffectParameter m_TextureParam;

        public EffectWithTexture(GraphicsDevice device, byte[] bytecode)
            : base(device, bytecode)
        {
            CacheEffectParameters();
        }

        protected EffectWithTexture(EffectWithTexture cloneSource)
            : base(cloneSource)
        {
            CacheEffectParameters();
        }

        public Texture2D Texture
        {
            get
            {
                return this.m_TextureParam.GetValueTexture2D();
            }

            set
            {
                this.m_TextureParam.SetValue(value);
            }
        }

        public override Effect Clone()
        {
            return new EffectWithTexture(this);
        }

        private void CacheEffectParameters()
        {
            this.m_TextureParam = this.Parameters["Texture"];
        }
    }
}
