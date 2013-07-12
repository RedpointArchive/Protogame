using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IRenderUtilities
    {
        /// <summary>
        /// Starts a specific render mode.
        /// </summary>
        /// <returns>The render mode.</returns>
        /// <param name="context">The rendering context.</param>
        /// <param name="mode">The new rendering mode to use.</param>
        IRenderState BeginRenderMode(IRenderContext context, RenderMode mode);
        
        /// <summary>
        /// Renders text at the specified position.
        /// </summary>
        /// <param name="context">The rendering context.</param>
        /// <param name="position">The position to render the text.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="font">The font to use for rendering.</param>
        /// <param name="horizontalAlignment">The horizontal alignment of the text (defaults to Left).</param>
        /// <param name="verticalAlignment">The vertical alignment of the text (defaults to Top).</param>
        /// <param name="textColor">The text color (defaults to white).</param>
        /// <param name="renderShadow">Whether to render a shadow on the text (defaults to true).</param>
        /// <param name="shadowColor">The text shadow's color (defaults to black).</param>
        void RenderText(
            IRenderContext context,
            Vector2 position,
            string text,
            FontAsset font,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment verticalAlignment = VerticalAlignment.Top,
            Color? textColor = null,
            bool renderShadow = true,
            Color? shadowColor = null);
        
        /// <summary>
        /// Renders a texture at the specified position.
        /// </summary>
        /// <param name="context">The rendering context.</param>
        /// <param name="position">The position to render the texture.</param>
        /// <param name="textureName">The texture name.</param>
        /// <param name="size">The size to render the texture as (defauls to the texture size).</param>
        /// <param name="color">The colorization to apply to the texture.</param>
        /// <param name="flipHorizontally">If set to <c>true</c> the texture is flipped horizontally.</param>
        /// <param name="flipVertically">If set to <c>true</c> the texture is flipped vertically.</param>
        void RenderTexture(
            IRenderContext context,
            Vector2 position,
            TextureAsset texture,
            Vector2? size = null,
            Color? color = null,
            bool flipHorizontally = false,
            bool flipVertically = false);
        
        /// <summary>
        /// Renders a 2D line.
        /// </summary>
        /// <param name="context">The rendering context.</param>
        /// <param name="start">The start of the line.</param>
        /// <param name="end">The end of the line.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="width">The width of the line (defaults to 1).</param>
        void RenderLine(
            IRenderContext context,
            Vector2 start,
            Vector2 end,
            Color color,
            float width = 1f);
        
        /// <summary>
        /// Renders a rectangle.
        /// </summary>
        /// <param name="context">The rendering context.</param>
        /// <param name="rectangle">The rectangle to render.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="filled">If set to <c>true</c>, the rectangle is rendered filled.</param>
        void RenderRectangle(
            IRenderContext context,
            Rectangle rectangle,
            Color color,
            bool filled = false);
    }
}

