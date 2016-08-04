using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// Provides basic graphics blitting functionality.
    /// <para>
    /// This services provides a <see cref="Blit"/> method, which can be used to copy the
    /// contents of one render target to another render target (or the backbuffer), optionally
    /// using a different shader.
    /// </para>
    /// </summary>
    /// <module>Graphics</module>
    public interface IGraphicsBlit
    {
        /// <summary>
        /// Blits a render target onto another render target (or the backbuffer), using either
        /// the default blit shader, or a specific effect for post-processing render passes.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="source">The source render target.  If null, does not load a source texture.</param>
        /// <param name="destination">The destination render target, or the current render target if null.</param>
        /// <param name="shader">The effect shader to use, or the default blit shader if null.</param>
        /// <param name="effectParameterSet">The effect parameters to use, or the default parameter set if null.</param>
        /// <param name="blendState">The blend state to use, or opaque blend mode if null.</param>
        /// <param name="offset">The top left position on the target. (0, 0) is top left, (1, 1) is bottom right.</param>
        /// <param name="size">The size of the render onto the target. (1, 1) is the full size of the target.</param>
        void Blit(
            IRenderContext renderContext, 
            Texture2D source, 
            RenderTarget2D destination = null, 
            IEffect shader = null,
            IEffectParameterSet effectParameterSet = null,
            BlendState blendState = null,
            Vector2? offset = null,
            Vector2? size = null);
    }
}