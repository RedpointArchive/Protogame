#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX

namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// An implementation of <see cref="IGameWindow"/> that provides support for desktop platforms.
    /// </summary>
    public class DefaultGameWindow : IGameWindow
    {
        /// <summary>
        /// Stores a reference to the underlying XNA game window.
        /// </summary>
        private readonly GameWindow m_GameWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="Protogame.DefaultGameWindow"/> class.
        /// </summary>
        /// <param name="gameWindow">
        /// The underlying XNA game window to wrap.
        /// </param>
        public DefaultGameWindow(GameWindow gameWindow)
        {
            this.m_GameWindow = gameWindow;
        }

        /// <summary>
        /// Gets the client bounds of the window.
        /// </summary>
        /// <value>
        /// The client bounds of the window.
        /// </value>
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
        /// <value>
        /// The title shown on the window.
        /// </value>
        public string Title
        {
            get
            {
                return this.m_GameWindow.Title;
            }

            set
            {
                this.m_GameWindow.Title = value;
            }
        }
    }
}

#endif