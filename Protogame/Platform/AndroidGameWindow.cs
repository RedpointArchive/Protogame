#if PLATFORM_ANDROID || PLATFORM_OUYA
namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    using AndroidMonoGameWindow = Microsoft.Xna.Framework.AndroidGameWindow;

    // <summary>
    /// An implementation of <see cref="IGameWindow"/> that provides support for the Android platform.
    /// </summary>
    public class AndroidGameWindow : IGameWindow
    {
        /// <summary>
        /// Stores a reference to the underlying Android game window.
        /// </summary>
        private AndroidMonoGameWindow m_GameWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="Protogame.Platform.AndroidGameWindow"/> class.
        /// </summary>
        /// <param name="gameWindow">The underlying Android game window to wrap.</param>
        public AndroidGameWindow(AndroidMonoGameWindow gameWindow)
        {
            this.m_GameWindow = gameWindow;
        }

        /// <summary>
        /// Gets the client bounds of the window.
        /// </summary>
        /// <value>The client bounds of the window.</value>
        public Rectangle ClientBounds
        {
            get
            {
                return this.m_GameWindow.ClientBounds;
            }
        }

        /// <summary>
        /// Gets or sets the title of the window. Ignored on platforms where
        /// the title is non-existent (such as mobile platforms).
        /// </summary>
        /// <value>The title shown on the window.</value>
        public string Title
        {
            get
            {
                return string.Empty;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to resize the window.  Ignored
        /// on platforms where the window is of a fixed size.
        /// </summary>
        /// <value>
        /// Whether or not the user is allowed to resize the window.
        /// </value>
        public bool AllowUserResizing
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        public bool IsFullscreen => true;

        public void SetFullscreen(bool fullscreen)
        {
        }

        public void Resize(int width, int height)
        {
        }

        public void SetCursorPosition(int x, int y)
        {
        }

        public void GetCursorPosition(out int x, out int y)
        {
            x = 0;
            y = 0;
        }
    }
}
#endif