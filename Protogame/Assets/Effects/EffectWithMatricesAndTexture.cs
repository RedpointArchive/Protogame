namespace Protogame
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The effect with matrices and texture.
    /// </summary>
    public class EffectWithMatricesAndTexture : Effect, IEffectMatrices, IEffectTexture
    {
        /// <summary>
        /// The m_ projection.
        /// </summary>
        private Matrix m_Projection = Matrix.Identity;

        /// <summary>
        /// The m_ texture param.
        /// </summary>
        private EffectParameter m_TextureParam;

        /// <summary>
        /// The m_ view.
        /// </summary>
        private Matrix m_View = Matrix.Identity;

        /// <summary>
        /// The m_ world.
        /// </summary>
        private Matrix m_World = Matrix.Identity;

        /// <summary>
        /// The m_ world view proj param.
        /// </summary>
        private EffectParameter m_WorldViewProjParam;

        /// <summary>
        /// The m_ world view proj param dirty.
        /// </summary>
        private bool m_WorldViewProjParamDirty = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectWithMatricesAndTexture"/> class.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        /// <param name="bytecode">
        /// The bytecode.
        /// </param>
        public EffectWithMatricesAndTexture(GraphicsDevice device, byte[] bytecode)
            : base(device, bytecode)
        {
            this.CacheEffectParameters();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectWithMatricesAndTexture"/> class.
        /// </summary>
        /// <param name="cloneSource">
        /// The clone source.
        /// </param>
        protected EffectWithMatricesAndTexture(EffectWithMatricesAndTexture cloneSource)
            : base(cloneSource)
        {
            this.CacheEffectParameters();

            this.m_World = cloneSource.m_World;
            this.m_View = cloneSource.m_View;
            this.m_Projection = cloneSource.m_Projection;
        }

        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
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

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
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

        /// <summary>
        /// Gets or sets the view matrix.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
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
        /// Gets or sets the world matrix.
        /// </summary>
        /// <value>
        /// The world.
        /// </value>
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
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="Effect"/>.
        /// </returns>
        public override Effect Clone()
        {
            return new EffectWithMatricesAndTexture(this);
        }

        /// <summary>
        /// The on apply.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

        /// <summary>
        /// The cache effect parameters.
        /// </summary>
        private void CacheEffectParameters()
        {
            this.m_TextureParam = this.Parameters["Texture"];
            this.m_WorldViewProjParam = this.Parameters["WorldViewProj"];
        }
    }
}