using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Protogame
{
    public interface ICoreGame
    {
        IGameWindow Window { get; }

        GraphicsDevice GraphicsDevice { get; }

        GraphicsDeviceManager GraphicsDeviceManager { get; }

        IGameContext GameContext { get; }
        
        IRenderContext RenderContext { get; }
        
        IUpdateContext UpdateContext { get; }

        bool IsMouseVisible { get; set; }

        void AssignHost(HostGame hostGame);

        void LoadContent();

        void UnloadContent();

        void DeviceLost();

        void DeviceResetting();

        void DeviceReset();

        void ResourceCreated(object resource);

        void ResourceDestroyed(string name, object tag);

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);

        void Dispose(bool disposing);

        /// <summary>
        /// Prepares the graphics device manager.
        /// <para>
        /// If you want to change the initial size of the game window on startup, this is the place
        /// to do it.  Override this method, and then set <see cref="Microsoft.Xna.Framework.GraphicsDeviceManager.PreferredBackBufferWidth"/>
        /// and <see cref="Microsoft.Xna.Framework.GraphicsDeviceManager.PreferredBackBufferHeight"/>.
        /// </para>
        /// </summary>
        /// <param name="graphicsDeviceManager">The graphics device manager to prepare.</param>
        void PrepareGraphicsDeviceManager(GraphicsDeviceManager graphicsDeviceManager);

        /// <summary>
        /// Prepares the game window.
        /// <para>
        /// If you want to change the initial position of the game window on startup, this is the place
        /// to do it.  Override this method, and then set the window properties.
        /// </para>
        /// </summary>
        /// <param name="window">The game window to prepare.</param>
        void PrepareGameWindow(IGameWindow window);

        /// <summary>
        /// Prepares the graphics devices settings.
        /// </summary>
        /// <remarks>
        /// The default configuration is to enable multisampling.  To disable multisampling,
        /// override PrepareDeviceSettings in your derived class.
        /// </remarks>
        /// <param name="deviceInformation">The device information.</param>
        void PrepareDeviceSettings(GraphicsDeviceInformation deviceInformation);

        void CloseRequested(out bool cancel);

        void Exit();
    }
}