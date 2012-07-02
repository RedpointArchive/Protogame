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
        private Texture2D m_Pixel = null;

        public XnaGraphics(GameContext context)
        {
            this.m_Context = context;
            this.m_Pixel = new Texture2D(context.Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this.m_Pixel.SetData(new[] { Color.White });
        }

        public void DrawStringLeft(int x, int y, string text)
        {
            this.DrawStringLeft(x, y, text, "Arial");
        }

        public void DrawStringLeft(int x, int y, string text, string font)
        {
            this.m_Context.SpriteBatch.DrawString(this.m_Context.Fonts[font], text, new Vector2((int)(x + 1), (int)(y + 1)), Color.Black);
            this.m_Context.SpriteBatch.DrawString(this.m_Context.Fonts[font], text, new Vector2((int)x, (int)y), Color.White);
        }

        public void DrawStringCentered(int x, int y, string text)
        {
            this.DrawStringCentered(x, y, text, "Arial");
        }

        public void DrawStringCentered(int x, int y, string text, string font)
        {
            Vector2 size = this.m_Context.Fonts[font].MeasureString(text);
            this.m_Context.SpriteBatch.DrawString(this.m_Context.Fonts[font], text, new Vector2((int)(x - size.X / 2 + 1), (int)(y + 1)), Color.Black);
            this.m_Context.SpriteBatch.DrawString(this.m_Context.Fonts[font], text, new Vector2((int)(x - size.X / 2), (int)y), Color.White);
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

        public void DrawLine(float x1, float y1, float x2, float y2, float width, Color color)
        {
            float angle = (float)Math.Atan2(y2 - y1, x2 - x1);
            float length = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2));

            this.m_Context.SpriteBatch.Draw(this.m_Pixel, new Vector2(x1, y1), null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }

        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            this.DrawRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color);
        }

        public void DrawRectangle(Vector2 from, Vector2 to, Color color)
        {
            this.DrawRectangle(from.X, from.Y, to.X - from.X, to.Y - from.Y, color);
        }

        public void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            this.DrawLine(x, y, x + width, y, 1, color);
            this.DrawLine(x, y, x, y + height, 1, color);
            this.DrawLine(x, y + height, x + width, y + height, 1, color);
            this.DrawLine(x + width, y, x + width, y + height, 1, color);
        }

        public void FillRectangle(Rectangle rectangle, Color color)
        {
            this.FillRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color);
        }

        public void FillRectangle(Vector2 from, Vector2 to, Color color)
        {
            this.FillRectangle(from.X, from.Y, to.X - from.X, to.Y - from.Y, color);
        }

        public void FillRectangle(float x, float y, float width, float height, Color color)
        {
            this.m_Context.SpriteBatch.Draw(this.m_Pixel, new Vector2(x, y), null, color,
                       0, Vector2.Zero, new Vector2(width, height),
                       SpriteEffects.None, 0);
        }
    }
}
