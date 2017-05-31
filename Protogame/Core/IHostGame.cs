using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Protogame
{
    public interface IHostGame
    {
        bool IsMouseVisible { get; set; }

        GraphicsDevice GraphicsDevice { get; }

        GraphicsDeviceManager GraphicsDeviceManager { get; }

        IGameWindow ProtogameWindow { get; }

        event EventHandler<EventArgs> Exiting;

        GameWindow Window { get; }

        GameServiceContainer Services { get; }

        ContentManager Content { get; set; }

        void Exit();

        SpriteBatch SplashScreenSpriteBatch { get; }

        Texture2D SplashScreenTexture { get; }
    }
}
