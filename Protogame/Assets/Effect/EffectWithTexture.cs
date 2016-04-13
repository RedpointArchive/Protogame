namespace Protogame
{
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The effect with texture.
    /// </summary>
    public class EffectWithTexture : Effect, IEffectTexture
    {
        /// <summary>
        /// The m_ texture param.
        /// </summary>
        private EffectParameter m_TextureParam;

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectWithTexture"/> class.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        /// <param name="bytecode">
        /// The bytecode.
        /// </param>
        public EffectWithTexture(GraphicsDevice device, byte[] bytecode)
            : base(device, bytecode)
        {
            this.CacheEffectParameters();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectWithTexture"/> class.
        /// </summary>
        /// <param name="cloneSource">
        /// The clone source.
        /// </param>
        protected EffectWithTexture(EffectWithTexture cloneSource)
            : base(cloneSource)
        {
            this.CacheEffectParameters();
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
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="Effect"/>.
        /// </returns>
        public override Effect Clone()
        {
            return new EffectWithTexture(this);
        }

        /// <summary>
        /// The cache effect parameters.
        /// </summary>
        private void CacheEffectParameters()
        {
            this.m_TextureParam = this.Parameters["Texture"];
        }
    }
}