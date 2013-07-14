using Microsoft.Xna.Framework;
using Ninject;
using Ninject.Parameters;

namespace Protogame
{
    public abstract class CoreGame<TInitialWorld, TWorldManager> : Game, ICoreGame where TInitialWorld : IWorld where TWorldManager : IWorldManager
    {
        public IGameContext GameContext { get; private set; }
        public IUpdateContext UpdateContext { get; private set; }
        public IRenderContext RenderContext { get; private set; }
        
        //protected GameContext m_GameContext = null;
        private int m_TotalFrames = 0;
        private float m_ElapsedTime = 0.0f;

        public CoreGame(IKernel kernel)
        {
            // TODO: Fix this because it means we can't have more than one
            // game using the same IoC container.
            var assetContentManager = new AssetContentManager(this.Services);
            this.Content = assetContentManager;
            kernel.Bind<IAssetContentManager>().ToMethod(x => assetContentManager);
            
            // Create the game context.
            this.GameContext = kernel.Get<IGameContext>(
                new ConstructorArgument("game", this),
                new ConstructorArgument("graphics", new GraphicsDeviceManager(this)),
                new ConstructorArgument("world", kernel.Get<TInitialWorld>()),
                new ConstructorArgument("worldManager", kernel.Get<TWorldManager>()),
                new ConstructorArgument("window", this.Window));
            
            // Create the update and render contexts.
            this.UpdateContext = kernel.Get<IUpdateContext>();
            this.RenderContext = kernel.Get<IRenderContext>();
            
            // Set up defaults.
            this.Window.Title = "Protogame!";
        }
        
        /// <summary>
        /// Creates a new world using IoC.
        /// </summary>
        /// <returns>The world from io c.</returns>
        /// <param name="worldType">World type.</param>
       /* private World CreateWorldFromIoC(Type worldType)
        {
            return this.m_Kernel.Get(worldType) as World;
        }*/

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }
        
        /// <summary>
        /// Forces the game to switch to a new world type, automatically creating the world
        /// via IoC so that constructor parameters are resolved.
        /// </summary>
        /// <returns>The world.</returns>
        /// <typeparam name="T">The type of the world.</typeparam>
        /*public World SwitchWorld<T>() where T : World
        {
            var world = this.CreateWorldFromIoC(typeof(T));
            this.SwitchWorld(world);
            return world;
        }

        /// <summary>
        /// Forces the game to switch to a new world type.  Should be used sparingly as it requires
        /// discarding and reloading all entities and tilesets.
        /// </summary>
        /// <param name="w">The world to switch to.</param>
        private void SwitchWorld(World w)
        {
            if (this.World != null)
                this.World.Game = null;
            this.m_GameContext.World = w;
            this.World.GameContext = this.m_GameContext;
            this.World.Game = this;
        }*/

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //this.m_GameContext.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
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
            //if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    this.Exit();

            // Measure FPS.
            this.GameContext.FrameCount += 1;
            this.m_ElapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (this.m_ElapsedTime >= 1000f)
            {
                this.GameContext.FPS = this.m_TotalFrames;
                this.m_TotalFrames = 0;
                this.m_ElapsedTime = 0;
            }

            // If this is before the 60th frame, skip so that MonoGame
            // can initialize properly.
            if (this.GameContext.FrameCount < 60)
                return;

            // Update the game.
            this.GameContext.GameTime = gameTime;
            this.GameContext.WorldManager.Update(this);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.m_TotalFrames++;

            // Skip if we haven't yet loaded the sprite batch.
            //if (this.m_GameContext.SpriteBatch == null)
            //    throw new ProtogameException("The sprite batch instance was not set when it came time to draw the game.  Ensure that you are calling base.LoadContent in the overridden LoadContent method of your game.");

            // If this is before the 60th frame, skip so that MonoGame
            // can initialize properly.
            if (this.GameContext.FrameCount < 60)
            {
                this.GraphicsDevice.Clear(Color.Black);
                return;
            }

            // Render the game.
            this.GameContext.GameTime = gameTime;
            this.GameContext.WorldManager.Render(this);

            base.Draw(gameTime);
        }
    }
}
