#pragma warning disable CS1591

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX || PLATFORM_WEB || PLATFORM_IOS

namespace Protogame
{
    /// <summary>
    /// An implementation of <see cref="IGameWindow"/> that provides support for desktop platforms.
    /// </summary>
    public class DefaultGameWindow : IGameWindow
    {
        private readonly HostGame _hostGame;
        private readonly GameWindow _gameWindow;

        public DefaultGameWindow(HostGame hostGame, GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
        }
        
        public Rectangle ClientBounds
        {
            get
            {
                return _gameWindow.ClientBounds;
            }
        }
        
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

        public bool IsFullscreen => _hostGame.GraphicsDeviceManager.IsFullScreen;

        public void Resize(int width, int height)
        {
            _hostGame.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            _hostGame.GraphicsDeviceManager.PreferredBackBufferHeight = height;
            _hostGame.GraphicsDeviceManager.ApplyChanges();
        }

        public void SetFullscreen(bool fullscreen)
        {
            _hostGame.GraphicsDeviceManager.HardwareModeSwitch = false;
            _hostGame.GraphicsDeviceManager.IsFullScreen = fullscreen;
            _hostGame.GraphicsDeviceManager.ApplyChanges();
        }

        public void SetCursorPosition(int x, int y)
        {
            Mouse.SetPosition(x, y);
        }

        public void GetCursorPosition(out int x, out int y)
        {
            var state = Mouse.GetState();
            x = state.X;
            y = state.Y;
        }

#if PLATFORM_WINDOWS
        public void Maximize()
        {
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(_gameWindow.Handle);
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }

        public void Minimize()
        {
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(_gameWindow.Handle);
            form.WindowState = System.Windows.Forms.FormWindowState.Minimized;
        }

        public void Restore()
        {
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(_gameWindow.Handle);
            form.WindowState = System.Windows.Forms.FormWindowState.Normal;
        }
#endif
    }
}

#endif