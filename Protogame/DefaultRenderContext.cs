using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    class DefaultRenderContext : IRenderContext
    {
        private Effect m_Effect;
        private BoundingFrustum m_BoundingFrustum;
    
        public GraphicsDevice GraphicsDevice { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public Texture2D SingleWhitePixel { get; private set; }
        public Effect Effect { get { return this.m_Effect; } }
        public BoundingFrustum BoundingFrustrum { get { return this.m_BoundingFrustum; } }

        public void SetEffect<T>(T effect) where T : Effect
        {
            this.m_Effect = effect;
        }

        private Matrix GetEffectMatrix(Func<IEffectMatrices, Matrix> prop)
        {
            var effectMatrices = this.m_Effect as IEffectMatrices;

            if (effectMatrices != null)
                return prop(effectMatrices);
            else
                return Matrix.Identity;
        }

        private void SetEffectMatrix(Action<IEffectMatrices> assign)
        {
            var effectMatrices = this.m_Effect as IEffectMatrices;

            if (effectMatrices != null)
                assign(effectMatrices);
        }

        public Matrix View
        {
            get
            {
                return this.GetEffectMatrix(x => x.View);
            }
            set
            {
                this.SetEffectMatrix(x => x.View = value);
                this.RecalculateBoundingFrustrum();
            }
        }
        
        public Matrix World
        {
            get
            {
                return this.GetEffectMatrix(x => x.World);
            }
            set
            {
                this.SetEffectMatrix(x => x.World = value);
            }
        }
        
        public Matrix Projection
        {
            get
            {
                return this.GetEffectMatrix(x => x.Projection);
            }
            set
            {
                this.SetEffectMatrix(x => x.Projection = value);
                this.RecalculateBoundingFrustrum();
            }
        }

        public bool Is3DContext { get; set; }
        
        private void RecalculateBoundingFrustrum()
        {
            this.m_BoundingFrustum = new BoundingFrustum(this.View * this.Projection);
        }
        
        public void Render(IGameContext context)
        {
            this.GraphicsDevice = context.Graphics.GraphicsDevice;
            if (this.m_Effect == null)
                this.m_Effect = new BasicEffect(context.Graphics.GraphicsDevice);
            if (this.SpriteBatch == null)
                this.SpriteBatch = new SpriteBatch(context.Graphics.GraphicsDevice);
            if (this.SingleWhitePixel == null)
            {
                this.SingleWhitePixel = new Texture2D(context.Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                this.SingleWhitePixel.SetData(new[] { Color.White });
            }
        }
        
        public void EnableTextures()
        {
            var basicEffect = this.m_Effect as BasicEffect;

            if (basicEffect != null)
            {
                basicEffect.TextureEnabled = true;
                basicEffect.VertexColorEnabled = false;
            }
        }
        
        public void EnableVertexColors()
        {
            var basicEffect = this.m_Effect as BasicEffect;

            if (basicEffect != null)
            {
                basicEffect.VertexColorEnabled = true;
                basicEffect.TextureEnabled = false;
            }
        }
        
        public void SetActiveTexture(Texture2D texture)
        {
            var basicEffect = this.m_Effect as BasicEffect;

            if (basicEffect != null)
            {
                basicEffect.Texture = texture;
            }
        }
    }
}

