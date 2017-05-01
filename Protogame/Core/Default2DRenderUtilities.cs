// ReSharper disable CheckNamespace
#pragma warning disable 1591

namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An implementation of <see cref="I2DRenderUtilities"/>.
    /// </summary>
    /// <module>Graphics 2D</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.I2DRenderUtilities</interface_ref>
    public class Default2DRenderUtilities : I2DRenderUtilities
    {
        private readonly IStringSanitizer _stringSanitizer;

        public Default2DRenderUtilities(
            IStringSanitizer stringSanitizer)
        {
            _stringSanitizer = stringSanitizer;
        }
        
        public Vector2 MeasureText(IRenderContext context, string text, IAssetReference<FontAsset> font)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return new Vector2(0, 0);
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            if (!font.IsReady)
            {
                return new Vector2(0, 0);
            }

            return font.Asset.Font.MeasureString(_stringSanitizer.SanitizeCharacters(font.Asset.Font, text));
        }
        
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
                RenderLine(
                    context, 
                    new Vector2(rectangle.X, rectangle.Y), 
                    new Vector2(rectangle.X + rectangle.Width, rectangle.Y), 
                    color);
                RenderLine(
                    context, 
                    new Vector2(rectangle.X, rectangle.Y), 
                    new Vector2(rectangle.X, rectangle.Y + rectangle.Height), 
                    color);
                RenderLine(
                    context, 
                    new Vector2(rectangle.X, rectangle.Y + rectangle.Height), 
                    new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), 
                    color);
                RenderLine(
                    context, 
                    new Vector2(rectangle.X + rectangle.Width, rectangle.Y), 
                    new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), 
                    color);
            }
        }
        
        public void RenderText(
            IRenderContext context, 
            Vector2 position, 
            string text, 
            IAssetReference<FontAsset> font, 
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, 
            VerticalAlignment verticalAlignment = VerticalAlignment.Top, 
            Color? textColor = null, 
            bool renderShadow = true, 
            Color? shadowColor = null,
            float? rotation = null,
            Vector2? origin = null,
            Vector2? scale = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            if (!font.IsReady)
            {
                return;
            }

            if (textColor == null)
            {
                textColor = Color.White;
            }

            if (shadowColor == null)
            {
                shadowColor = Color.Black;
            }

            if (font.Asset.Font == null)
            {
                throw new AssetNotCompiledException(font.Asset.Name);
            }

            // Sanitize the text.
            text = _stringSanitizer.SanitizeCharacters(font.Asset.Font, text);

            // Determine position to draw.
            var size = font.Asset.Font.MeasureString(text);
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
                context.SpriteBatch.DrawString(font.Asset.Font, text, new Vector2(xx, yy) + (scale ?? Vector2.One), shadowColor.Value, rotation ?? 0, origin ?? Vector2.Zero, scale ?? Vector2.One, SpriteEffects.None, 0);
            }

            // Render the main text.
            context.SpriteBatch.DrawString(font.Asset.Font, text, new Vector2(xx, yy), textColor.Value, rotation ?? 0, origin ?? Vector2.Zero, scale ?? Vector2.One, SpriteEffects.None, 0);
        }
        
        public void RenderTexture(
            IRenderContext context, 
            Vector2 position, 
            IAssetReference<TextureAsset> texture, 
            Vector2? size = null, 
            Color? color = null, 
            float rotation = 0,
            Vector2? rotationAnchor = null,
            bool flipHorizontally = false, 
            bool flipVertically = false, 
            Rectangle? sourceArea = null)
        {
            if (!texture.IsReady)
            {
                return;
            }

            RenderTexture(
                context,
                position,
                texture.Asset.Texture,
                size ?? new Vector2(texture.Asset.OriginalWidth, texture.Asset.OriginalHeight),
                color,
                rotation,
                rotationAnchor,
                flipHorizontally,
                flipVertically,
                sourceArea);
        }
        
        public void RenderTexture(
            IRenderContext context,
            Vector2 position,
            Texture2D texture,
            Vector2? size = null,
            Color? color = null,
            float rotation = 0,
            Vector2? rotationAnchor = null,
            bool flipHorizontally = false,
            bool flipVertically = false,
            Rectangle? sourceArea = null)
        {
            if (size == null)
            {
                size = new Vector2(texture.Width, texture.Height);
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
                texture,
                new Rectangle((int)position.X, (int)position.Y, (int)size.Value.X, (int)size.Value.Y),
                sourceArea,
                color.Value.ToPremultiplied(),
                rotation,
                rotationAnchor ?? Vector2.Zero,
                effects,
                0);
        }
        
        public void RenderCircle(
            IRenderContext context,
            Vector2 center, 
            int radius, 
            Color color, 
            bool filled = false)
        {
            var points = 360;

            double angle = MathHelper.TwoPi / points;

            for (int i = 1; i <= points; i++)
            {
                var pos = new Vector2(
                    (float)Math.Round(Math.Sin(angle * i), 4) * radius,
                    (float)Math.Round(Math.Cos(angle * i), 4) * radius);
                var nextPos = new Vector2(
                    (float)Math.Round(Math.Sin(angle * (i + 1)), 4) * radius,
                    (float)Math.Round(Math.Cos(angle * (i + 1)), 4) * radius);

                RenderLine(
                    context,
                    center + pos,
                    center + nextPos,
                    Color.White);
            }
        }
        
        public void SuspendSpriteBatch(IRenderContext renderContext)
        {
            renderContext.SpriteBatch.End();
        }
        
        public void ResumeSpriteBatch(IRenderContext renderContext)
        {
            renderContext.SpriteBatch.Begin();
        }
    }
}