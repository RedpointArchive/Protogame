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

        /// <summary>
        /// Resize the game window to the specified dimensions.
        /// </summary>
        /// <param name="width">The new width of the window.</param>
        /// <param name="height">The new height of the window.</param>
        void Resize(int width, int height);

        /// <summary>
        /// Gets whether or not the game is currently in fullscreen mode.
        /// </summary>
        bool IsFullscreen { get; }

        /// <summary>
        /// Turns fullscreen mode on or off immediately.
        /// </summary>
        /// <param name="fullscreen">True if the game should be fullscreen, false otherwise.</param>
        void SetFullscreen(bool fullscreen);

        /// <summary>
        /// Sets the position of the mouse cursor on the screen, relative to the window.
        /// </summary>
        /// <param name="x">The horizontal position of the cursor.</param>
        /// <param name="y">The vertical position of the cursor.</param>
        void SetCursorPosition(int x, int y);
        
        /// <summary>
        /// Gets the position of the mouse cursor on the screen, relative to the window.
        /// </summary>
        /// <param name="x">The horizontal position of the cursor.</param>
        /// <param name="y">The vertical position of the cusor.</param>
        void GetCursorPosition(out int x, out int y);
        
#if PLATFORM_WINDOWS
        void Maximize();

        void Minimize();

        void Restore();
#endif
    }
}