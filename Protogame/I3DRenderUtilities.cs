using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface I3DRenderUtilities
    {
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
            Matrix matrix,
            string text,
            FontAsset font,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment verticalAlignment = VerticalAlignment.Top,
            Color? textColor = null,
            bool renderShadow = true,
            Color? shadowColor = null);
        
        /// <summary>
        /// Measures text as if it was rendered with the font asset.
        /// </summary>
        /// <param name="context">The rendering context.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="font">The font to use for rendering.</param>
        Vector2 MeasureText(
            IRenderContext context,
            string text,
            FontAsset font);
        
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
            Matrix matrix,
            TextureAsset texture,
            Color? color = null,
            bool flipHorizontally = false,
            bool flipVertically = false,
            Rectangle? sourceArea = null);
        
        /// <summary>
        /// Renders a 3D line.
        /// </summary>
        /// <param name="context">The rendering context.</param>
        /// <param name="start">The start of the line.</param>
        /// <param name="end">The end of the line.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="width">The width of the line (defaults to 1).</param>
        void RenderLine(
            IRenderContext context,
            Vector3 start,
            Vector3 end,
            Color color);
        
        /// <summary>
        /// Renders a rectangle.
        /// </summary>
        /// <param name="context">The rendering context.</param>
        /// <param name="start">The top, left, position of the rectangle.</param>
        /// <param name="end">The bottom, right position of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="filled">If set to <c>true</c>, the rectangle is rendered filled.</param>
        void RenderRectangle(
            IRenderContext context,
            Vector3 start,
            Vector3 end,
            Color color,
            bool filled = false);
    }
}

