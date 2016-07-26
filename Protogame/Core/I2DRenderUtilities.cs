using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The 2DRenderUtilities interface.
    /// </summary>
    /// <module>Graphics 2D</module>
    public interface I2DRenderUtilities
    {
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
        Vector2 MeasureText(IRenderContext context, string text, FontAsset font);

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
        void RenderLine(IRenderContext context, Vector2 start, Vector2 end, Color color, float width = 1f);

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
        void RenderRectangle(IRenderContext context, Rectangle rectangle, Color color, bool filled = false);

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
        /// <param name="rotationAnchor">
        /// The anchor for rotation, or <c>null</c> to use the top-left corner.
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
        void RenderTexture(
            IRenderContext context, 
            Vector2 position, 
            TextureAsset texture, 
            Vector2? size = null, 
            Color? color = null, 
            float rotation = 0, 
            Vector2? rotationAnchor = null,
            bool flipHorizontally = false, 
            bool flipVertically = false, 
            Rectangle? sourceArea = null);

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
        /// <param name="rotationAnchor">
        /// The anchor for rotation, or <c>null</c> to use the top-left corner.
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
        void RenderTexture(
            IRenderContext context,
            Vector2 position,
            Texture2D texture,
            Vector2? size = null,
            Color? color = null,
            float rotation = 0,
            Vector2? rotationAnchor = null,
            bool flipHorizontally = false,
            bool flipVertically = false,
            Rectangle? sourceArea = null);

        /// <summary>
        /// Renders a circle.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <param name="center">
        /// The center of the circle.
        /// </param>
        /// <param name="radius">
        /// The radius of the circle.
        /// </param>
        /// <param name="color">
        /// The color of the circle.
        /// </param>
        /// <param name="filled">
        /// If set to <c>true</c>, the circle is rendered filled.
        /// </param>
        void RenderCircle(
            IRenderContext context,
            Vector2 center, 
            int radius, 
            Color color, 
            bool filled = false);

        /// <summary>
        /// Suspends usage of the sprite batch so that direct rendering can occur during a 2D context.
        /// </summary>
        /// <param name="renderContext">
        /// The current rendering context.
        /// </param>
        void SuspendSpriteBatch(IRenderContext renderContext);

        /// <summary>
        /// Resumes usage of the sprite batch again.
        /// </summary>
        /// <param name="renderContext">
        /// The current rendering context.
        /// </param>
        void ResumeSpriteBatch(IRenderContext renderContext);
    }
}