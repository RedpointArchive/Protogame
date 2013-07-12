using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    class DefaultRenderContext : IRenderContext
    {
        public SpriteBatch SpriteBatch { get; private set; }
        public Texture2D SingleWhitePixel { get; private set; }
        
        public void Render(IGameContext context)
        {
            if (this.SpriteBatch == null)
                this.SpriteBatch = new SpriteBatch(context.Graphics.GraphicsDevice);
            if (this.SingleWhitePixel == null)
            {
                this.SingleWhitePixel = new Texture2D(context.Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                this.SingleWhitePixel.SetData(new[] { Color.White });
            }
        }
    }
}

