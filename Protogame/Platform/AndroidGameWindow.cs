#if PLATFORM_ANDROID || PLATFORM_OUYA

namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    using AndroidMonoGameWindow = Microsoft.Xna.Framework.AndroidGameWindow;

    /// <summary>
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
                return "";
            }

            set
            {
            }
        }
    }
}

#endif
