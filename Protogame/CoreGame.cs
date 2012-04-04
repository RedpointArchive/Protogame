using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public abstract class CoreGame<T> : Game where T : World, new()
    {
        protected GameContext m_GameContext = null;
        private WorldManager m_WorldManager = null;

        public T World
        {
            get
            {
                return this.m_GameContext.World as T;
            }
        }
        
        public CoreGame()
        {
            this.Content.RootDirectory = "Content";
            this.m_GameContext = new GameContext
            {
                Content = this.Content,
                World = new T(),
                Graphics = new GraphicsDeviceManager(this)
            };
            this.m_WorldManager = new WorldManager();
            this.World.GameContext = this.m_GameContext;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.Window.Title = "Protogame!";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.m_GameContext.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected sealed override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // Update the game.
            this.m_WorldManager.Update(this.m_GameContext);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected sealed override void Draw(GameTime gameTime)
        {
            // Skip if we haven't yet loaded the sprite batch.
            if (this.m_GameContext.SpriteBatch == null)
                throw new ProtogameException("The sprite batch instance was not set when it came time to draw the game.  Ensure that you are calling base.LoadContent in the overridden LoadContent method of your game.");

            // Draw the game.
            this.m_WorldManager.Draw(this.m_GameContext);

            base.Draw(gameTime);
        }
    }
}
