using Microsoft.Xna.Framework;
using Ninject;
using Ninject.Parameters;

namespace Protogame
{
    public abstract class CoreGame<TInitialWorld, TWorldManager> : Game, ICoreGame where TInitialWorld : IWorld where TWorldManager : IWorldManager
    {
        private IKernel m_Kernel;
        private int m_TotalFrames = 0;
        private float m_ElapsedTime = 0.0f;
        private GraphicsDeviceManager m_GraphicsDeviceManager;
    
        public IGameContext GameContext { get; private set; }
        public IUpdateContext UpdateContext { get; private set; }
        public IRenderContext RenderContext { get; private set; }

        public CoreGame(IKernel kernel)
        {
            this.m_Kernel = kernel;
            this.m_GraphicsDeviceManager = new GraphicsDeviceManager(this);
        
            // TODO: Fix this because it means we can't have more than one game using the same IoC container.
            var assetContentManager = new AssetContentManager(this.Services);
            this.Content = assetContentManager;
            kernel.Bind<IAssetContentManager>().ToMethod(x => assetContentManager);
        }
        
        protected override void LoadContent()
        {
            // Create the game context.
            this.GameContext = this.m_Kernel.Get<IGameContext>(
                new ConstructorArgument("game", this),
                new ConstructorArgument("graphics", this.m_GraphicsDeviceManager),
                new ConstructorArgument("world", this.m_Kernel.Get<TInitialWorld>()),
                new ConstructorArgument("worldManager", this.m_Kernel.Get<TWorldManager>()),
                new ConstructorArgument("window", this.Window));
            
            // Create the update and render contexts.
            this.UpdateContext = this.m_Kernel.Get<IUpdateContext>();
            this.RenderContext = this.m_Kernel.Get<IRenderContext>();
            
            // Set up defaults.
            this.Window.Title = "Protogame!";
        }

        protected override void Update(GameTime gameTime)
        {
            // Measure FPS.
            this.GameContext.FrameCount += 1;
            this.m_ElapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (this.m_ElapsedTime >= 1000f)
            {
                this.GameContext.FPS = this.m_TotalFrames;
                this.m_TotalFrames = 0;
                this.m_ElapsedTime = 0;
            }

            // If this is before the 60th frame, skip so that MonoGame can initialize properly.
            if (this.GameContext.FrameCount < 60)
                return;

            // Update the game.
            this.GameContext.GameTime = gameTime;
            this.GameContext.WorldManager.Update(this);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.m_TotalFrames++;

            // If this is before the 60th frame, skip so that MonoGame can initialize properly.
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
