using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class EffectWithMatricesAndTexture : Effect, IEffectMatrices, IEffectTexture
    {
        private Matrix m_World = Matrix.Identity;
        private Matrix m_View = Matrix.Identity;
        private Matrix m_Projection = Matrix.Identity;

        private EffectParameter m_TextureParam;
        private EffectParameter m_WorldViewProjParam;
        private bool m_WorldViewProjParamDirty = true;

        public EffectWithMatricesAndTexture(GraphicsDevice device, byte[] bytecode)
            : base(device, bytecode)
        {
            CacheEffectParameters();
        }

        protected EffectWithMatricesAndTexture(EffectWithMatricesAndTexture cloneSource)
            : base(cloneSource)
        {
            CacheEffectParameters();

            this.m_World = cloneSource.m_World;
            this.m_View = cloneSource.m_View;
            this.m_Projection = cloneSource.m_Projection;
        }

        /// <summary>
        /// Gets or sets the world matrix.
        /// </summary>
        public Matrix World
        {
            get
            {
                return this.m_World;
            }

            set
            {
                this.m_World = value;
                this.m_WorldViewProjParamDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the view matrix.
        /// </summary>
        public Matrix View
        {
            get
            {
                return this.m_View;
            }

            set
            {
                this.m_View = value;
                this.m_WorldViewProjParamDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        public Matrix Projection
        {
            get
            {
                return this.m_Projection;
            }

            set
            {
                this.m_Projection = value;
                this.m_WorldViewProjParamDirty = true;
            }
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
            return new EffectWithMatricesAndTexture(this);
        }

        protected override bool OnApply()
        {
            if (this.m_WorldViewProjParamDirty)
            {
                Matrix worldViewProj;
                Matrix worldView;

                Matrix.Multiply(ref this.m_World, ref this.m_View, out worldView);
                Matrix.Multiply(ref worldView, ref this.m_Projection, out worldViewProj);

                this.m_WorldViewProjParam.SetValue(worldViewProj);

                this.m_WorldViewProjParamDirty = false;
            }

            return false;
        }

        private void CacheEffectParameters()
        {
            this.m_TextureParam = this.Parameters["Texture"];
            this.m_WorldViewProjParam = this.Parameters["WorldViewProj"];
        }
    }
}
