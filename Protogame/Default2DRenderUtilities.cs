namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An implementation of <see cref="I2DRenderUtilities"/>.
    /// </summary>
    public class Default2DRenderUtilities : I2DRenderUtilities
    {
        /// <summary>
        /// The string sanitizer used to sanitize text before it is rendered.
        /// </summary>
        private readonly IStringSanitizer m_StringSanitizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Default2DRenderUtilities"/> class.
        /// </summary>
        /// <param name="stringSanitizer">
        /// The dependency injected <see cref="IStringSanitizer"/> instance.
        /// </param>
        public Default2DRenderUtilities(IStringSanitizer stringSanitizer)
        {
            this.m_StringSanitizer = stringSanitizer;
        }

        /// <summary>
        /// Measures text as if it was rendered with the font asset.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <param name="text">
        /// The text to render.
        /// </param>
        /// <param name="font">
        /// The font to use for rendering.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/>.
        /// </returns>
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

            return font.Font.MeasureString(this.m_StringSanitizer.SanitizeCharacters(font.Font, text));
        }

        /// <summary>
        /// Renders a 2D line.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <param name="start">
        /// The start of the line.
        /// </param>
        /// <param name="end">
        /// The end of the line.
        /// </param>
        /// <param name="color">
        /// The color of the line.
        /// </param>
        /// <param name="width">
        /// The width of the line (defaults to 1).
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
        /// Renders a rectangle.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <param name="rectangle">
        /// The rectangle to render.
        /// </param>
        /// <param name="color">
        /// The color of the rectangle.
        /// </param>
        /// <param name="filled">
        /// If set to <c>true</c>, the rectangle is rendered filled.
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
        /// Renders text at the specified position.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <param name="position">
        /// The position to render the text.
        /// </param>
        /// <param name="text">
        /// The text to render.
        /// </param>
        /// <param name="font">
        /// The font to use for rendering.
        /// </param>
        /// <param name="horizontalAlignment">
        /// The horizontal alignment of the text (defaults to Left).
        /// </param>
        /// <param name="verticalAlignment">
        /// The vertical alignment of the text (defaults to Top).
        /// </param>
        /// <param name="textColor">
        /// The text color (defaults to white).
        /// </param>
        /// <param name="renderShadow">
        /// Whether to render a shadow on the text (defaults to true).
        /// </param>
        /// <param name="shadowColor">
        /// The text shadow's color (defaults to black).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the render context or font asset is null.
        /// </exception>
        /// <exception cref="AssetNotCompiledException">
        /// Thrown if the font asset has not been compiled.
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

            // Sanitize the text.
            text = this.m_StringSanitizer.SanitizeCharacters(font.Font, text);

            // Determine position to draw.
            var size = font.Font.MeasureString(text);
            float xx = 0, yy = 0;
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    xx = position.X;
                    break;
                case HorizontalAlignment.Center:
                    xx = position.X - (size.X / 2);
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
                    yy = position.Y - (size.Y / 2);
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
        /// Renders a texture at the specified position.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <param name="position">
        /// The position to render the texture.
        /// </param>
        /// <param name="texture">
        /// The texture.
        /// </param>
        /// <param name="size">
        /// The size to render the texture as (defaults to the texture size).
        /// </param>
        /// <param name="color">
        /// The colorization to apply to the texture.
        /// </param>
        /// <param name="rotation">
        /// The rotation to apply to the texture.
        /// </param>
        /// <param name="flipHorizontally">
        /// If set to <c>true</c> the texture is flipped horizontally.
        /// </param>
        /// <param name="flipVertically">
        /// If set to <c>true</c> the texture is flipped vertically.
        /// </param>
        /// <param name="sourceArea">
        /// The source area of the texture (defaults to the full texture).
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

            var effects =
                (SpriteEffects)
                ((int)(flipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None)
                 + (int)(flipVertically ? SpriteEffects.FlipVertically : SpriteEffects.None));

            context.SpriteBatch.Draw(
                texture.Texture, 
                new Rectangle((int)position.X, (int)position.Y, (int)size.Value.X, (int)size.Value.Y), 
                sourceArea, 
                color.Value.ToPremultiplied(), 
                rotation,
                new Vector2(0, 0),
                effects, 
                0);
        }

        /// <summary>
        /// Suspends usage of the sprite batch so that direct rendering can occur during a 2D context.
        /// </summary>
        /// <param name="renderContext">
        /// The current rendering context.
        /// </param>
        public void SuspendSpriteBatch(IRenderContext renderContext)
        {
            renderContext.SpriteBatch.End();
        }

        /// <summary>
        /// Resumes usage of the sprite batch again.
        /// </summary>
        /// <param name="renderContext">
        /// The current rendering context.
        /// </param>
        public void ResumeSpriteBatch(IRenderContext renderContext)
        {
            renderContext.SpriteBatch.Begin();
        }
    }
}