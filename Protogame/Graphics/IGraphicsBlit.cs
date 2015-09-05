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
        /// <param name="source">The source render target.</param>
        /// <param name="destination">The destination render target, or the backbuffer if null.</param>
        /// <param name="shader">The effect shader to use, or the default blit shader if null.</param>
        void Blit(IRenderContext renderContext, RenderTarget2D source, RenderTarget2D destination = null, Effect shader = null);
    }
}