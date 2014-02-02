namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The BasicSkin interface.
    /// </summary>
    public interface IBasicSkin
    {
        /// <summary>
        /// Gets the back surface color.
        /// </summary>
        /// <value>
        /// The back surface color.
        /// </value>
        Color BackSurfaceColor { get; }

        /// <summary>
        /// Gets the dark edge color.
        /// </summary>
        /// <value>
        /// The dark edge color.
        /// </value>
        Color DarkEdgeColor { get; }

        /// <summary>
        /// Gets the dark surface color.
        /// </summary>
        /// <value>
        /// The dark surface color.
        /// </value>
        Color DarkSurfaceColor { get; }

        /// <summary>
        /// Gets the light edge color.
        /// </summary>
        /// <value>
        /// The light edge color.
        /// </value>
        Color LightEdgeColor { get; }

        /// <summary>
        /// Gets the surface color.
        /// </summary>
        /// <value>
        /// The surface color.
        /// </value>
        Color SurfaceColor { get; }

        /// <summary>
        /// Gets the text color.
        /// </summary>
        /// <value>
        /// The text color.
        /// </value>
        Color TextColor { get; }
    }
}