using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private IProfiler m_Profiler;
        private IEngineHook[] m_EngineHooks;
    
        public IGameContext GameContext { get; private set; }
        public IUpdateContext UpdateContext { get; private set; }
        public IRenderContext RenderContext { get; private set; }

        public CoreGame(IKernel kernel)
        {
            this.m_Kernel = kernel;
            this.m_GraphicsDeviceManager = new GraphicsDeviceManager(this);
            this.m_GraphicsDeviceManager.PreparingDeviceSettings += (sender, e) => 
            {
                e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };
            this.m_Profiler = kernel.TryGet<IProfiler>();
            if (this.m_Profiler == null)
            {
                kernel.Bind<IProfiler>().To<NullProfiler>();
                this.m_Profiler = kernel.Get<IProfiler>();
            }
        
            // TODO: Fix this because it means we can't have more than one game using the same IoC container.
            var assetContentManager = new AssetContentManager(this.Services);
            this.Content = assetContentManager;
            kernel.Bind<IAssetContentManager>().ToMethod(x => assetContentManager);
        }
        
        protected override void LoadContent()
        {
            // The interception library can't properly intercept class types, which
            // means we can't simply do this.m_Kernel.Get<TInitialWorld>() because
            // none of the calls will be intercepted.  Instead, we need to bind the
            // IWorld and IWorldManager to their initial types and then unbind them
            // after they've been constructed.
            this.m_Kernel.Bind<IWorld>().To<TInitialWorld>();
            this.m_Kernel.Bind<IWorldManager>().To<TWorldManager>();
            var world = this.m_Kernel.Get<IWorld>();
            var worldManager = this.m_Kernel.Get<IWorldManager>();
            this.m_Kernel.Unbind<IWorld>();
            this.m_Kernel.Unbind<IWorldManager>();
        
            // Create the game context.
            this.GameContext = this.m_Kernel.Get<IGameContext>(
                new ConstructorArgument("game", this),
                new ConstructorArgument("graphics", this.m_GraphicsDeviceManager),
                new ConstructorArgument("world", world),
                new ConstructorArgument("worldManager", worldManager),
                new ConstructorArgument("window", this.Window));
            
            // Create the update and render contexts.
            this.UpdateContext = this.m_Kernel.Get<IUpdateContext>();
            this.RenderContext = this.m_Kernel.Get<IRenderContext>();
            
            // Retrieve all engine hooks.  These can be set up by additional modules
            // to change runtime behaviour.
            this.m_EngineHooks = this.m_Kernel.GetAll<IEngineHook>().ToArray();
            
            // Set up defaults.
            this.Window.Title = "Protogame!";
        }

        protected override void Update(GameTime gameTime)
        {
            using (this.m_Profiler.Measure("update", this.GameContext.FrameCount.ToString()))
            {
                // Measure FPS.
                using (this.m_Profiler.Measure("measure_fps"))
                {
                    this.GameContext.FrameCount += 1;
                    this.m_ElapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (this.m_ElapsedTime >= 1000f)
                    {
                        this.GameContext.FPS = this.m_TotalFrames;
                        this.m_TotalFrames = 0;
                        this.m_ElapsedTime = 0;
                    }
                }
    
                // If this is before the 60th frame, skip so that MonoGame can initialize properly.
                if (this.GameContext.FrameCount < 60)
                    return;
    
                // Update the game.
                using (this.m_Profiler.Measure("main"))
                {
                    this.GameContext.GameTime = gameTime;
                    foreach (var hook in this.m_EngineHooks)
                        hook.Update(this.GameContext, this.UpdateContext);
                    this.GameContext.WorldManager.Update(this);
                }
    
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            using (this.m_Profiler.Measure("render", (this.GameContext.FrameCount - 1).ToString()))
            {
                this.m_TotalFrames++;
    
                // If this is before the 60th frame, skip so that MonoGame can initialize properly.
                if (this.GameContext.FrameCount < 60)
                {
                    this.GraphicsDevice.Clear(Color.Black);
                    return;
                }
    
                // Render the game.
                using (this.m_Profiler.Measure("main"))
                {
                    this.GameContext.GameTime = gameTime;
                    foreach (var hook in this.m_EngineHooks)
                        hook.Render(this.GameContext, this.RenderContext);
                    this.GameContext.WorldManager.Render(this);
                }
    
                base.Draw(gameTime);
            }
        }
    }
}
