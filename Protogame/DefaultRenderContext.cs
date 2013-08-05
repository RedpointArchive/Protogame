using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    class DefaultRenderContext : IRenderContext
    {
        private BasicEffect m_Effect;
    
        public GraphicsDevice GraphicsDevice { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public Texture2D SingleWhitePixel { get; private set; }
        public Effect Effect { get { return this.m_Effect; } }
        
        public Matrix View
        {
            get { return this.m_Effect.View; }
            set { this.m_Effect.View = value; }
        }
        
        public Matrix World
        {
            get { return this.m_Effect.View; }
            set { this.m_Effect.World = value; }
        }
        
        public Matrix Projection
        {
            get { return this.m_Effect.View; }
            set { this.m_Effect.Projection = value; }
        }
        
        public bool Is3DContext { get; set; }
        
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
        }
        
        public void EnableVertexColors()
        {
            this.m_Effect.VertexColorEnabled = true;
        }
        
        public void SetActiveTexture(Texture2D texture)
        {
            this.m_Effect.Texture = texture;
        }
    }
}

