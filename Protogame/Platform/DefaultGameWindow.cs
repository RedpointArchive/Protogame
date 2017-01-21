using Microsoft.Xna.Framework;

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX || PLATFORM_WEB || PLATFORM_IOS

namespace Protogame
{
    /// <summary>
    /// An implementation of <see cref="IGameWindow"/> that provides support for desktop platforms.
    /// </summary>
    public class DefaultGameWindow : IGameWindow
    {
        /// <summary>
        /// Stores a reference to the underlying XNA game window.
        /// </summary>
        private readonly GameWindow _gameWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="Protogame.DefaultGameWindow"/> class.
        /// </summary>
        /// <param name="gameWindow">
        /// The underlying XNA game window to wrap.
        /// </param>
        public DefaultGameWindow(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
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
                return _gameWindow.ClientBounds;
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
                return _gameWindow.Title;
            }

            set
            {
                _gameWindow.Title = value;
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
                return _gameWindow.AllowUserResizing;
            }

            set
            {
                // We check whether the value is already the desired value, because setting
                // this property triggers a window resize event even if the value is the same.
                if (_gameWindow.AllowUserResizing != value)
                {
                    _gameWindow.AllowUserResizing = value;
                }
            }
        }

        /// <summary>
        /// The underlying MonoGame window object.  The type and presence of this property
        /// varies per-platform, so it's access should always be within a platform specific block.
        /// </summary>
        public GameWindow PlatformWindow
        {
            get { return _gameWindow; }
        }
    }
}

#endif