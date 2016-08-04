namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The 3DRenderUtilities interface.
    /// </summary>
    /// <module>Graphics 3D</module>
    public interface I3DRenderUtilities
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
        /// Renders a 3D line.
        /// </summary>
        /// <param name="context">
        ///     The rendering context.
        /// </param>
        /// <param name="start">
        ///     The start of the line.
        /// </param>
        /// <param name="effectParameterSet"></param>
        /// <param name="end">
        ///     The end of the line.
        /// </param>
        /// <param name="color">
        ///     The color of the line.
        /// </param>
        /// <param name="effect"></param>
        void RenderLine(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Vector3 start, Vector3 end, Color color);

        /// <summary>
        /// Renders a 3D line using texture UVs.
        /// </summary>
        /// <param name="context">
        ///     The rendering context.
        /// </param>
        /// <param name="start">
        ///     The start of the line.
        /// </param>
        /// <param name="effectParameterSet"></param>
        /// <param name="end">
        ///     The end of the line.
        /// </param>
        /// <param name="texture">
        ///     The texture to use.
        /// </param>
        /// <param name="startUV">
        ///     The UV for the start of the line.
        /// </param>
        /// <param name="endUV">
        ///     The UV for the end of the line.
        /// </param>
        /// <param name="effect"></param>
        void RenderLine(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Vector3 start, Vector3 end, TextureAsset texture, Vector2 startUV, Vector2 endUV);

        /// <summary>
        /// Renders a rectangle.
        /// </summary>
        /// <param name="context">
        ///     The rendering context.
        /// </param>
        /// <param name="start">
        ///     The top, left, position of the rectangle.
        /// </param>
        /// <param name="effectParameterSet"></param>
        /// <param name="end">
        ///     The bottom, right position of the rectangle.
        /// </param>
        /// <param name="color">
        ///     The color of the rectangle.
        /// </param>
        /// <param name="filled">
        ///     If set to <c>true</c>, the rectangle is rendered filled.
        /// </param>
        /// <param name="effect"></param>
        void RenderRectangle(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Vector3 start, Vector3 end, Color color, bool filled = false);

        /// <summary>
        /// Renders text at the specified position.
        /// </summary>
        /// <param name="context">
        ///     The rendering context.
        /// </param>
        /// <param name="matrix">
        ///     The matrix.
        /// </param>
        /// <param name="effectParameterSet"></param>
        /// <param name="text">
        ///     The text to render.
        /// </param>
        /// <param name="font">
        ///     The font to use for rendering.
        /// </param>
        /// <param name="horizontalAlignment">
        ///     The horizontal alignment of the text (defaults to Left).
        /// </param>
        /// <param name="verticalAlignment">
        ///     The vertical alignment of the text (defaults to Top).
        /// </param>
        /// <param name="textColor">
        ///     The text color (defaults to white).
        /// </param>
        /// <param name="renderShadow">
        ///     Whether to render a shadow on the text (defaults to true).
        /// </param>
        /// <param name="shadowColor">
        ///     The text shadow's color (defaults to black).
        /// </param>
        /// <param name="effect"></param>
        void RenderText(
            IRenderContext context,
            IEffect effect, 
            IEffectParameterSet effectParameterSet,
            Matrix matrix, 
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
        ///     The rendering context.
        /// </param>
        /// <param name="matrix">
        ///     The matrix.
        /// </param>
        /// <param name="effectParameterSet"></param>
        /// <param name="texture">
        ///     The texture.
        /// </param>
        /// <param name="color">
        ///     The colorization to apply to the texture.
        /// </param>
        /// <param name="flipHorizontally">
        ///     If set to <c>true</c> the texture is flipped horizontally.
        /// </param>
        /// <param name="flipVertically">
        ///     If set to <c>true</c> the texture is flipped vertically.
        /// </param>
        /// <param name="sourceArea">
        ///     The source Area.
        /// </param>
        /// <param name="effect"></param>
        void RenderTexture(
            IRenderContext context,
            IEffect effect,
            IEffectParameterSet effectParameterSet,
            Matrix matrix, 
            TextureAsset texture, 
            Color? color = null, 
            bool flipHorizontally = false, 
            bool flipVertically = false, 
            Rectangle? sourceArea = null);

        /// <summary>
        /// Renders a 3D cube from 0, 0, 0 to 1, 1, 1, applying the specified transformation.
        /// </summary>
        /// <param name="context">
        ///     The rendering context.
        /// </param>
        /// <param name="transform">
        ///     The transformation to apply.
        /// </param>
        /// <param name="effectParameterSet"></param>
        /// <param name="color">
        ///     The color of the cube.
        /// </param>
        /// <param name="effect"></param>
        void RenderCube(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform, Color color);

        /// <summary>
        /// Renders a 3D cube from 0, 0, 0 to 1, 1, 1, applying the specified transformation, with the
        /// given texture and using the specified UV coordinates for each face of the cube.
        /// </summary>
        /// <param name="context">
        ///     The rendering context.
        /// </param>
        /// <param name="transform">
        ///     The transformation to apply.
        /// </param>
        /// <param name="effectParameterSet"></param>
        /// <param name="texture">
        ///     The texture to render on the cube.
        /// </param>
        /// <param name="topLeftUV">
        ///     The top-left UV coordinate.
        /// </param>
        /// <param name="bottomRightUV">
        ///     The bottom-right UV coordinate.
        /// </param>
        /// <param name="effect"></param>
        void RenderCube(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform, TextureAsset texture, Vector2 topLeftUV, Vector2 bottomRightUV);

        /// <summary>
        /// Renders a 2D plane from 0, 0 to 1, 1, applying the specified transformation.
        /// </summary>
        /// <param name="context">
        ///     The rendering context.
        /// </param>
        /// <param name="transform">
        ///     The transformation to apply.
        /// </param>
        /// <param name="effectParameterSet"></param>
        /// <param name="color">
        ///     The color of the plane.
        /// </param>
        /// <param name="effect"></param>
        void RenderPlane(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform, Color color);

        /// <summary>
        /// Renders a 2D plane from 0, 0 to 1, 1, applying the specified transformation.
        /// </summary>
        /// <param name="context">
        ///     The rendering context.
        /// </param>
        /// <param name="transform">
        ///     The transformation to apply.
        /// </param>
        /// <param name="effectParameterSet"></param>
        /// <param name="texture">
        ///     The texture to render on the plane.
        /// </param>
        /// <param name="topLeftUV">
        ///     The top-left UV coordinate.
        /// </param>
        /// <param name="bottomRightUV">
        ///     The bottom-right UV coordinate.
        /// </param>
        /// <param name="effect"></param>
        void RenderPlane(IRenderContext context, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform, TextureAsset texture, Vector2 topLeftUV, Vector2 bottomRightUV);

        /// <summary>
        /// Renders a circle.
        /// </summary>
        /// <param name="context">
        ///     The rendering context.
        /// </param>
        /// <param name="matrix"></param>
        /// <param name="effectParameterSet"></param>
        /// <param name="center">
        ///     The center of the circle.
        /// </param>
        /// <param name="radius">
        ///     The radius of the circle.
        /// </param>
        /// <param name="color">
        ///     The color of the circle.
        /// </param>
        /// <param name="filled">
        ///     If set to <c>true</c>, the circle is rendered filled.
        /// </param>
        /// <param name="effect"></param>
        void RenderCircle(
            IRenderContext context,
            IEffect effect, 
            IEffectParameterSet effectParameterSet,
            Matrix matrix, 
            Vector2 center,
            int radius,
            Color color,
            bool filled = false);
    }
}