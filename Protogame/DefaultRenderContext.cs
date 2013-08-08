using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    class DefaultRenderContext : IRenderContext
    {
        private BasicEffect m_Effect;
        private BoundingFrustum m_BoundingFrustum;
    
        public GraphicsDevice GraphicsDevice { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public Texture2D SingleWhitePixel { get; private set; }
        public Effect Effect { get { return this.m_Effect; } }
        public BoundingFrustum BoundingFrustrum { get { return this.m_BoundingFrustum; } }
        
        public Matrix View
        {
            get { return this.m_Effect.View; }
            set { this.m_Effect.View = value; this.RecalculateBoundingFrustrum(); }
        }
        
        public Matrix World
        {
            get { return this.m_Effect.View; }
            set { this.m_Effect.World = value; this.RecalculateBoundingFrustrum(); }
        }
        
        public Matrix Projection
        {
            get { return this.m_Effect.View; }
            set { this.m_Effect.Projection = value; }
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
            this.m_Effect.TextureEnabled = true;
            this.m_Effect.VertexColorEnabled = false;
        }
        
        public void EnableVertexColors()
        {
            this.m_Effect.VertexColorEnabled = true;
            this.m_Effect.TextureEnabled = false;
        }
        
        public void SetActiveTexture(Texture2D texture)
        {
            this.m_Effect.Texture = texture;
        }
    }
}

