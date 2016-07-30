using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// Indicates that the render pass has a configurable viewport, which can be used
    /// for split-screen games.  The render pipeline optimizes viewport switching to
    /// ensure that unnecessary GPU calls are not made when the viewport remains the
    /// same between render passes.
    /// </summary>
    public interface IRenderPassWithViewport
    {
        /// <summary>
        /// Gets or sets the viewport used in this rendering pass.
        /// <para>
        /// By configuring different viewports on multiple render passes, you
        /// can easily configure split-screen games, where different viewports
        /// are used for different players.  If this value is null, the whole
        /// screen is used.
        /// </para>
        /// </summary>
        /// <value>The viewport used for rendering.</value>
        Viewport? Viewport { get; set; }
    }
}
