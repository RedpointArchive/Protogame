using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Protogame.Platformer;

namespace Example
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ExampleGame : PlatformerGame<ExampleWorld>
    {
        public ExampleGame()
        {
            this.m_GameContext.Graphics.PreferredBackBufferWidth = 800;
            this.m_GameContext.Graphics.PreferredBackBufferHeight = 600;

            // Load our initial level.
            this.World.LoadLevel("Level0");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.Window.Title = "Protogame Example!";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load protogame's content.
            base.LoadContent();

            // Load all the textures.
            this.m_GameContext.LoadFont("Arial");
            this.m_GameContext.LoadTexture("tiles");
            this.m_GameContext.LoadTexture("black"); // Needed for FadingWorld to work (you can remove this if you don't use FadingWorld).
            this.m_GameContext.LoadTextureAnim("player.frame", 5);
            this.m_GameContext.LoadAudio("audio.sfx.example");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            base.UnloadContent();
        }
    }
}
