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
using Protogame.RTS;
using OgmoEditor.Protogame;

namespace Example.RTS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class RuntimeGame : RTSGame<MyRTSWorld>
    {
        public RuntimeGame()
            : this("Level0")
        {
        }

        public RuntimeGame(string level)
        {
            this.m_GameContext.Graphics.PreferredBackBufferWidth = 800;
            this.m_GameContext.Graphics.PreferredBackBufferHeight = 600;

            // Load our initial level.
            this.World.LoadMultiLevel(level);

            // Set the Ogmo Editor to focus on this object.
            OgmoConnect.FocusedObject = new OgmoState(this);
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
            this.IsMouseVisible = true;
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
            this.m_GameContext.LoadFont("BigArial");
            this.m_GameContext.LoadTexture("SurfaceTiles");
            this.m_GameContext.LoadTexture("CircleUnit");
            this.m_GameContext.LoadTexture("units.infantry.builder");
            this.m_GameContext.LoadTexture("units.infantry.heavy");
            this.m_GameContext.LoadTexture("units.infantry.medic");
            this.m_GameContext.LoadTexture("units.infantry.sniper");
            this.m_GameContext.LoadTexture("units.infantry.soldier");
            this.m_GameContext.LoadTexture("units.infantry.specialist");
            this.m_GameContext.LoadTexture("units.mechs.battlemech");
            this.m_GameContext.LoadTexture("units.mechs.constructionmech");
            this.m_GameContext.LoadTexture("units.mechs.heavymech");
            this.m_GameContext.LoadTexture("units.mechs.utilitymech");
            this.m_GameContext.LoadTextureAnim("meters.health.i", 16);
            this.m_GameContext.LoadTextureAnim("meters.meteorite.i", 16);
            this.m_GameContext.LoadTextureAnim("meters.meteorite.m", 23);
            this.m_GameContext.LoadTextureAnim("meters.radiation.i", 16);
            this.m_GameContext.LoadTextureAnim("meters.radiation.m", 23);
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
