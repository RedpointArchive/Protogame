namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The default 2 d render utilities.
    /// </summary>
    public class Default2DRenderUtilities : I2DRenderUtilities
    {
        /// <summary>
        /// The measure text.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public Vector2 MeasureText(IRenderContext context, string text, FontAsset font)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return new Vector2(0, 0);
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            return font.Font.MeasureString(text);
        }

        /// <summary>
        /// The render line.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        public void RenderLine(IRenderContext context, Vector2 start, Vector2 end, Color color, float width = 1f)
        {
            var angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            var length = Vector2.Distance(start, end);

            context.SpriteBatch.Draw(
                context.SingleWhitePixel, 
                new Vector2(start.X + 1, start.Y), 
                null, 
                color, 
                angle, 
                Vector2.Zero, 
                new Vector2(length, width), 
                SpriteEffects.None, 
                0);
        }

        /// <summary>
        /// The render rectangle.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="rectangle">
        /// The rectangle.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="filled">
        /// The filled.
        /// </param>
        public void RenderRectangle(IRenderContext context, Rectangle rectangle, Color color, bool filled = false)
        {
            if (filled)
            {
                context.SpriteBatch.Draw(
                    context.SingleWhitePixel, 
                    new Vector2(rectangle.X, rectangle.Y), 
                    null, 
                    color, 
                    0, 
                    Vector2.Zero, 
                    new Vector2(rectangle.Width, rectangle.Height), 
                    SpriteEffects.None, 
                    0);
            }
            else
            {
                this.RenderLine(
                    context, 
                    new Vector2(rectangle.X, rectangle.Y), 
                    new Vector2(rectangle.X + rectangle.Width, rectangle.Y), 
                    color);
                this.RenderLine(
                    context, 
                    new Vector2(rectangle.X, rectangle.Y), 
                    new Vector2(rectangle.X, rectangle.Y + rectangle.Height), 
                    color);
                this.RenderLine(
                    context, 
                    new Vector2(rectangle.X, rectangle.Y + rectangle.Height), 
                    new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), 
                    color);
                this.RenderLine(
                    context, 
                    new Vector2(rectangle.X + rectangle.Width, rectangle.Y), 
                    new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), 
                    color);
            }
        }

        /// <summary>
        /// The render text.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <param name="horizontalAlignment">
        /// The horizontal alignment.
        /// </param>
        /// <param name="verticalAlignment">
        /// The vertical alignment.
        /// </param>
        /// <param name="textColor">
        /// The text color.
        /// </param>
        /// <param name="renderShadow">
        /// The render shadow.
        /// </param>
        /// <param name="shadowColor">
        /// The shadow color.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="AssetNotCompiledException">
        /// </exception>
        public void RenderText(
            IRenderContext context, 
            Vector2 position, 
            string text, 
            FontAsset font, 
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, 
            VerticalAlignment verticalAlignment = VerticalAlignment.Top, 
            Color? textColor = null, 
            bool renderShadow = true, 
            Color? shadowColor = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            if (textColor == null)
            {
                textColor = Color.White;
            }

            if (shadowColor == null)
            {
                shadowColor = Color.Black;
            }

            if (font.Font == null)
            {
                throw new AssetNotCompiledException(font.Name);
            }

            // Determine position to draw.
            var size = font.Font.MeasureString(text);
            float xx = 0, yy = 0;
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    xx = position.X;
                    break;
                case HorizontalAlignment.Center:
                    xx = position.X - size.X / 2;
                    break;
                case HorizontalAlignment.Right:
                    xx = position.X - size.X;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    yy = position.Y;
                    break;
                case VerticalAlignment.Center:
                    yy = position.Y - size.Y / 2;
                    break;
                case VerticalAlignment.Bottom:
                    yy = position.Y - size.Y;
                    break;
            }

            // Normalize location to prevent blurring artifacts.
            xx = (int)xx;
            yy = (int)yy;

            // Draw shadow if required.
            if (renderShadow)
            {
                context.SpriteBatch.DrawString(font.Font, text, new Vector2(xx + 1, yy + 1), shadowColor.Value);
            }

            // Render the main text.
            context.SpriteBatch.DrawString(font.Font, text, new Vector2(xx, yy), textColor.Value);
        }

        /// <summary>
        /// The render texture.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="texture">
        /// The texture.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <param name="flipHorizontally">
        /// The flip horizontally.
        /// </param>
        /// <param name="flipVertically">
        /// The flip vertically.
        /// </param>
        /// <param name="sourceArea">
        /// The source area.
        /// </param>
        public void RenderTexture(
            IRenderContext context, 
            Vector2 position, 
            TextureAsset texture, 
            Vector2? size = null, 
            Color? color = null, 
            float rotation = 0, 
            bool flipHorizontally = false, 
            bool flipVertically = false, 
            Rectangle? sourceArea = null)
        {
            if (size == null)
            {
                size = new Vector2(texture.Texture.Width, texture.Texture.Height);
            }

            if (color == null)
            {
                color = Color.White;
            }

            context.SpriteBatch.Draw(
                texture.Texture, 
                new Rectangle((int)position.X, (int)position.Y, (int)size.Value.X, (int)size.Value.Y), 
                sourceArea, 
                color.Value.ToPremultiplied(), 
                rotation, 
                new Vector2(0, 0), 
                (SpriteEffects)
                ((int)(flipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None)
                 + (int)(flipVertically ? SpriteEffects.FlipVertically : SpriteEffects.None)), 
                0);
        }
    }
}