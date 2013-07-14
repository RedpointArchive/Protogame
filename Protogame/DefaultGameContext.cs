using Microsoft.Xna.Framework;
using Ninject;

namespace Protogame
{
    class DefaultGameContext : IGameContext
    {
        private IKernel m_Kernel;
    
        #if LEGACY
        public ContentManager Content { get; set; }
        public Dictionary<string, Texture2D> Textures { get; set; }
        public Dictionary<string, SpriteFont> Fonts { get; set; }
        public Dictionary<string, SoundEffect> Sounds { get; set; }
        public Dictionary<string, Effect> Effects { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        #endif
        public GraphicsDeviceManager Graphics { get; set; }
        public IWorld World { get; set; }
        public GameTime GameTime { get; set; }
        public Camera Camera { get; set; }
        public GameWindow Window { get; set; }
        public int FPS { get; set; }
        public Game Game { get; internal set; }
        public IWorldManager WorldManager { get; internal set; }
        public int FrameCount { get; set; }
        
        public DefaultGameContext(
            IKernel kernel,
            Game game,
            GraphicsDeviceManager graphics,
            GameWindow window,
            IWorld world,
            IWorldManager worldManager)
        {
            this.m_Kernel = kernel;
            this.Game = game;
            this.Graphics = graphics;
            this.World = world;
            this.WorldManager = worldManager;
            this.Window = window;
        }
        
        public void ResizeWindow(int width, int height)
        {
            if (this.Window.ClientBounds.Width == width &&
                this.Window.ClientBounds.Height == height)
                return;
            this.Graphics.PreferredBackBufferWidth = width;
            this.Graphics.PreferredBackBufferHeight = height;
            this.Camera = new Camera(width, height);
            this.Graphics.ApplyChanges();
        }

        #if LEGACY
        internal GameContext()
        {
            this.Textures = new Dictionary<string, Texture2D>();
            this.Sounds = new Dictionary<string, SoundEffect>();
            this.Fonts = new Dictionary<string, SpriteFont>();
            this.Effects = new Dictionary<string, Effect>();
        }

        // Copied from GameContext; TODO: unify this.
        public void EndSpriteBatch()
        {
            this.SpriteBatch.End();
        }

        // Copied from GameContext; TODO: unify this.
        public void StartSpriteBatch()
        {
            this.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                null,
                null,
                null,
                null,
                null,
                this.Camera.GetTransformationMatrix());
        }

        public Rectangle ScreenBounds
        {
            get { return this.Window.ClientBounds; }
        }

        public void SetScreenSize(int width, int height)
        {
            this.Graphics.PreferredBackBufferWidth = 1024; // GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.Graphics.PreferredBackBufferHeight = 768; // GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        public void LoadFont(string name)
        {
            this.Fonts.Add(name, this.Content.Load<SpriteFont>(name.Replace('.', '/')));
        }

        public void LoadTexture(string name)
        {
            this.Textures.Add(name, this.Content.Load<Texture2D>(name.Replace('.', '/')));
        }

        public void LoadTextureAnim(string name, int end)
        {
            for (int i = 1; i <= end; i++)
                this.LoadTexture(name + i);
        }

        public void LoadAudio(string name)
        {
            this.Sounds.Add(name, this.Content.Load<SoundEffect>(name.Replace('.', '/')));
        }

        public void LoadEffect(string name)
        {
            try
            {
                this.Effects.Add(name, this.Content.Load<Effect>(name.Replace('.', '/')));
            }
            catch (Exception)
            {
                this.Effects.Add(name, null);
            }
        }
        #endif

        public IWorld CreateWorld<T>() where T : IWorld
        {
            return this.m_Kernel.Get<T>();
        }
        
        public void SwitchWorld<T>() where T : IWorld
        {
            this.World = this.CreateWorld<T>();
        }
        
        public void SwitchWorld<T>(T world) where T : IWorld
        {
            this.World = world;
        }
    }
}
