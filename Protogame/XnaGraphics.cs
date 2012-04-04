using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class XnaGraphics
    {
        private GameContext m_Context = null;

        public XnaGraphics(GameContext context)
        {
            this.m_Context = context;
        }

        public void DrawStringLeft(int x, int y, string text)
        {
            this.m_Context.SpriteBatch.DrawString(this.m_Context.Fonts["Arial"], text, new Vector2(x + 1, y + 1), Color.Black);
            this.m_Context.SpriteBatch.DrawString(this.m_Context.Fonts["Arial"], text, new Vector2(x, y), Color.White);
        }

        public void DrawStringCentered(int x, int y, string text)
        {
            Vector2 size = this.m_Context.Fonts["Arial"].MeasureString(text);
            this.m_Context.SpriteBatch.DrawString(this.m_Context.Fonts["Arial"], text, new Vector2(x - size.X / 2 + 1, y + 1), Color.Black);
            this.m_Context.SpriteBatch.DrawString(this.m_Context.Fonts["Arial"], text, new Vector2(x - size.X / 2, y), Color.White);
        }

        public void DrawSprite(int x, int y, int width, int height, string image, Color color, bool flipX)
        {
            this.m_Context.SpriteBatch.Draw(
                this.m_Context.Textures[image],
                new Rectangle(x, y, width, height),
                null,
                color.ToPremultiplied(),
                0,
                new Vector2(0, 0),
                SpriteEffects.FlipHorizontally,
                0
                );
        }
    }
}
