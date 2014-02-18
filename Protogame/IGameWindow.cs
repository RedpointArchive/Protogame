namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// An interface that represents the game window in a cross-platform manner.
    /// </summary>
    public interface IGameWindow
    {
        /// <summary>
        /// Gets the client bounds of the window.
        /// </summary>
        /// <value>
        /// The client bounds of the window.
        /// </value>
        Rectangle ClientBounds { get; }

        /// <summary>
        /// Gets or sets the title of the window.  Ignored on platforms where
        /// the title is non-existent (such as mobile platforms).
        /// </summary>
        /// <value>
        /// The title shown on the window.
        /// </value>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to resize the window.  Ignored
        /// on platforms where the window is of a fixed size.
        /// </summary>
        /// <value>
        /// Whether or not the user is allowed to resize the window.
        /// </value>
        bool AllowUserResizing { get; set; }
    }
}