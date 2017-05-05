// ReSharper disable CheckNamespace

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protoinject;
using System.Diagnostics;

namespace Protogame
{
    /// <summary>
    /// The core Protogame game implementation.  You should derive your Game class from this
    /// implementation.
    /// </summary>
    /// <typeparam name="TInitialWorld">
    /// The initial world class to start the game with.
    /// </typeparam>
    /// <module>Core API</module>
    public abstract class CoreGame<TInitialWorld> : CoreGame<TInitialWorld, RenderPipelineWorldManager> where TInitialWorld : IWorld
    {
        /// <summary>
        /// The dependency injection kernel.
        /// </summary>
        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreGame{TInitialWorld}"/> class. 
        /// </summary>
        /// <param name="kernel">
        /// The dependency injection kernel.
        /// </param>
        protected CoreGame(IKernel kernel) : base(kernel)
        {
            _kernel = kernel;
        }

        /// <summary>
        /// Configure the render pipeline before the game begins.
        /// <para>
        /// In the new rendering system, you need to add render passes to the render pipeline
        /// of your game to indicate how things will be rendered.  Use
        /// <see cref="IRenderPipeline.AddFixedRenderPass"/> or
        /// <see cref="IRenderPipeline.AppendTransientRenderPass"/> to add passes to the render pipeline.
        /// </para>
        /// </summary>
        /// <param name="pipeline">The render pipeline to configure.</param>
        /// <param name="kernel">
        /// The dependency injection kernel, on which you can call <c>Get</c> on to create
        /// new render passes for adding to the pipeline.
        /// </param>
        protected abstract void ConfigureRenderPipeline(IRenderPipeline pipeline, IKernel kernel);
        
        /// <summary>
        /// Calls <see cref="ConfigureRenderPipeline"/>, which needs to be implemented by
        /// your game to configure the render pipeline.
        /// </summary>
        /// <param name="pipeline">The render pipeline to configure.</param>
        protected sealed override void InternalConfigureRenderPipeline(IRenderPipeline pipeline)
        {
            ConfigureRenderPipeline(pipeline, _kernel);
        }

    }

    /// <summary>
    /// The implementation of Protogame's base game class.  In previous versions of Protogame, this
    /// acted as the base class for the developer's Game class.  However newer games written in
    /// Protogame should use <see cref="CoreGame{TInitialWorld}"/> instead, as this correctly
    /// sets games to use the new render pipeline, which sets TWorldManager to be
    /// <see cref="RenderPipelineWorldManager"/> automatically.
    /// </summary>
    /// <typeparam name="TInitialWorld">
    /// The initial world class to start the game with.
    /// </typeparam>
    /// <typeparam name="TWorldManager">
    /// The world manager class for this game.
    /// </typeparam>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.CoreGame{TInitialWorld}</interface_ref>
    public abstract class CoreGame<TInitialWorld, TWorldManager> : ICoreGame
        where TInitialWorld : IWorld where TWorldManager : IWorldManager
    {
        /// <summary>
        /// The dependency injection kernel.
        /// </summary>
        private readonly IKernel _kernel;

        /// <summary>
        /// The current profiler instance.
        /// </summary>
        private readonly IProfiler _profiler;

        /// <summary>
        /// The current analytics engine instance.
        /// </summary>
        private readonly IAnalyticsEngine _analyticsEngine;

        /// <summary>
        /// The total number of frames that have elapsed since the last second interval.
        /// </summary>
        private int _totalFrames;

        /// <summary>
        /// The total amount of time that has elapsed since the last second interval.
        /// </summary>
        private float _elapsedTime;

        /// <summary>
        /// An array of the engine hooks that are loaded.
        /// </summary>
        private IEngineHook[] _engineHooks;

        /// <summary>
        /// The current hierarchical node of the game in the dependency injection system.
        /// </summary>
        private readonly INode _node;

        /// <summary>
        /// The asynchronous LoadContent task.
        /// </summary>
        private Task _loadContentTask;

        /// <summary>
        /// The coroutine scheduling service.
        /// </summary>
        private ICoroutine _coroutine;

        /// <summary>
        /// The coroutine scheduling execution service.
        /// </summary>
        private ICoroutineScheduler _coroutineScheduler;

        /// <summary>
        /// The loading screen used during early game startup.
        /// </summary>
        private ILoadingScreen _loadingScreen;

        /// <summary>
        /// The console handle used to emit early startup timing logs.
        /// </summary>
        private IConsoleHandle _consoleHandle;

        /// <summary>
        /// Whether we've done an initial early render.
        /// </summary>
        private bool _hasDoneEarlyRender;

        /// <summary>
        /// Whether we're ready for the world manager to start rendering.
        /// </summary>
        private bool _isReadyForMainRenderTakeover;

        /// <summary>
        /// Whether we've run LoadContentAsync at least once.
        /// </summary>
        private bool _hasDoneInitialLoadContent;

        /// <summary>
        /// The MonoGame game instance that is hosting this game.
        /// </summary>
        private HostGame _hostGame;

        /// <summary>
        /// Gets the current game context.  You should not generally access this property; outside
        /// an explicit Update or Render loop, the state of the game context is not guaranteed.  Inside
        /// the context of an Update or Render loop, the game context is already provided.
        /// </summary>
        /// <value>
        /// The current game context.
        /// </value>
        public IGameContext GameContext { get; private set; }

        /// <summary>
        /// Gets the current update context.  You should not generally access this property; outside
        /// an explicit Update loop, the state of the update context is not guaranteed.  Inside
        /// the context of an Update loop, the update context is already provided.
        /// </summary>
        /// <value>
        /// The current update context.
        /// </value>
        public IUpdateContext UpdateContext { get; private set; }

        /// <summary>
        /// Gets the current render context.  You should not generally access this property; outside
        /// an explicit Render loop, the state of the render context is not guaranteed.  Inside
        /// the context of an Render loop, the render context is already provided.
        /// </summary>
        /// <value>
        /// The current update context.
        /// </value>
        public IRenderContext RenderContext { get; private set; }

        public bool IsMouseVisible
        {
            get { return _hostGame?.IsMouseVisible ?? false; }
            set { if (_hostGame != null) { _hostGame.IsMouseVisible = value; } }
        }

        /// <summary>
        /// Initializes an instance of a game in Protogame.  This constructor is always called
        /// as the base constructor to your game implementation.
        /// </summary>
        /// <param name="kernel">The dependency injection kernel to use.</param>
        protected CoreGame(IKernel kernel)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

#if PLATFORM_MACOS
            // On Mac, the MonoGame launcher changes the current working
            // directory which means we can't find any assets.  Change it
            // back to where Protogame is located (because this is usually
            // beside the game).
            var directory = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
            if (directory != null)
            {
                System.Environment.CurrentDirectory = directory.FullName;
            }
#endif

            _kernel = kernel;
            _node = _kernel.CreateEmptyNode("Game");

            _profiler = kernel.TryGet<IProfiler>(_node);
            if (_profiler == null)
            {
                kernel.Bind<IProfiler>().To<NullProfiler>();
                _profiler = kernel.Get<IProfiler>(_node);
            }

            _analyticsEngine = kernel.TryGet<IAnalyticsEngine>(_node);
            if (_analyticsEngine == null)
            {
                kernel.Bind<IAnalyticsEngine>().To<NullAnalyticsEngine>();
                _analyticsEngine = kernel.Get<IAnalyticsEngine>(_node);
            }

            var analyticsInitializer = kernel.TryGet<IAnalyticsInitializer>(_node);
            if (analyticsInitializer == null)
            {
                kernel.Bind<IAnalyticsInitializer>().To<NullAnalyticsInitializer>();
                analyticsInitializer = kernel.Get<IAnalyticsInitializer>(_node);
            }

            _consoleHandle = kernel.TryGet<IConsoleHandle>(_node);
            
            StartupTrace.TimingEntries["constructOptionalGameDependencies"] = stopwatch.Elapsed;
            stopwatch.Restart();

            analyticsInitializer.Initialize(_analyticsEngine);
            
            StartupTrace.TimingEntries["initializeAnalytics"] = stopwatch.Elapsed;
            stopwatch.Restart();

            _coroutine = _kernel.Get<ICoroutine>();

            StartupTrace.TimingEntries["constructCoroutine"] = stopwatch.Elapsed;
            stopwatch.Restart();

            _coroutineScheduler = _kernel.Get<ICoroutineScheduler>();

            StartupTrace.TimingEntries["constructCoroutineScheduler"] = stopwatch.Elapsed;
            stopwatch.Restart();
        }

        /// <summary>
        /// A platform independent representation of a game window.
        /// </summary>
        /// <value>
        /// The game window.
        /// </value>
        public IGameWindow Window => _hostGame?.ProtogameWindow;

        /// <summary>
        /// The graphics device used by the game.
        /// </summary>
        /// <value>
        /// The graphics device.
        /// </value>
        public GraphicsDevice GraphicsDevice => _hostGame?.GraphicsDevice;

        /// <summary>
        /// The graphics device manager used by the game.
        /// </summary>
        /// <value>
        /// The graphics device manager.
        /// </value>
        public GraphicsDeviceManager GraphicsDeviceManager => _hostGame?.GraphicsDeviceManager;

        /// <summary>
        /// The number of frames to skip before updating or rendering.
        /// </summary>
        public int SkipFrames { get; set; }

        /// <summary>
        /// Called by <see cref="HostGame"/> to assign itself to this game
        /// instance, allowing us to access MonoGame game members.
        /// </summary>
        /// <param name="hostGame">The MonoGame game instance.</param>
        public void AssignHost(HostGame hostGame)
        {
            _hostGame = hostGame;

            var assetContentManager = new AssetContentManager(_hostGame.Services);
            _hostGame.Content = assetContentManager;
            _kernel.Bind<IAssetContentManager>().ToMethod(x => assetContentManager);

            // We can't load the loading screen until we have access to MonoGame's asset content manager.
            _loadingScreen = _kernel.Get<ILoadingScreen>();
        }
        
        public void LoadContent()
        {
            _consoleHandle.LogDebug("LoadContent called");

            _loadContentTask = _coroutine.Run(async () =>
            {
                await LoadContentAsync();
            });
        }

        public void UnloadContent()
        {
            _consoleHandle.LogDebug("UnloadContent called");
        }

        public void DeviceLost()
        {
            _consoleHandle.LogDebug("DeviceLost called");
        }

        public void DeviceResetting()
        {
            _consoleHandle.LogDebug("DeviceResetting called");
        }

        public void DeviceReset()
        {
            _consoleHandle.LogDebug("DeviceReset called");
        }

        public void ResourceCreated(object resource)
        {
            _consoleHandle.LogDebug("ResourceCreated called ({0})", resource);
        }

        public void ResourceDestroyed(string name, object tag)
        {
            _consoleHandle.LogDebug("ResourceDestroyed called ({0}, {1})", name, tag);
        }

        public void Exit()
        {
            _hostGame?.Exit();
        }

        protected virtual async Task LoadContentAsync()
        {
            if (!_hasDoneInitialLoadContent)
            {
                _hasDoneInitialLoadContent = true;

#if PLATFORM_ANDROID
                // On Android, disable viewport / backbuffer scaling because we expect games
                // to make use of the full display area.
                this.GraphicsDeviceManager.IsFullScreen = true;
                this.GraphicsDeviceManager.PreferredBackBufferHeight = this.Window.ClientBounds.Height;
                this.GraphicsDeviceManager.PreferredBackBufferWidth = this.Window.ClientBounds.Width;
#endif

#if PLATFORM_WINDOWS
                // Register for the window resize event so we can scale
                // the window correctly.
                var shouldHandleResize = true;
                _hostGame.Window.ClientSizeChanged += (sender, e) =>
                {
                    if (!shouldHandleResize)
                    {
                        return;
                    }

                    shouldHandleResize = false;
                    var width = _hostGame.Window.ClientBounds.Width;
                    var height = _hostGame.Window.ClientBounds.Height;
                    GameContext.Graphics.PreferredBackBufferWidth = width;
                    GameContext.Graphics.PreferredBackBufferHeight = height;
                    GameContext.Graphics.ApplyChanges();
                    shouldHandleResize = true;
                };

                // Register for the window close event so we can dispatch
                // it correctly.
                var form = System.Windows.Forms.Control.FromHandle(_hostGame.Window.Handle) as System.Windows.Forms.Form;
                if (form != null)
                {
                    form.FormClosing += (sender, args) =>
                    {
                        bool cancel;
                        CloseRequested(out cancel);

                        if (cancel)
                        {
                            args.Cancel = true;
                        }
                    };
                }
#endif

                // Allow the user to configure the game window now.
                PrepareGameWindow(Window);

                // Construct the world manager.
                var worldManager = await _kernel.GetAsync<TWorldManager>(_node, null, null, new IInjectionAttribute[0], new IConstructorArgument[0], null);

                // Create the game context.
                GameContext = await _kernel.GetAsync<IGameContext>(
                    _node,
                    null,
                    null,
                    new IInjectionAttribute[0],
                    new IConstructorArgument[]
                    {
                        new NamedConstructorArgument("game", this),
                        new NamedConstructorArgument("graphics", _hostGame.GraphicsDeviceManager),
                        new NamedConstructorArgument("world", null),
                        new NamedConstructorArgument("worldManager", worldManager),
                        new NamedConstructorArgument("window", _hostGame.ProtogameWindow)
                    }, null);

                // If we are using the new rendering pipeline, we need to ensure that
                // the rendering context and the render pipeline world manager share
                // the same render pipeline.
                var renderPipelineWorldManager = worldManager as RenderPipelineWorldManager;
                IRenderPipeline renderPipeline = null;
                if (renderPipelineWorldManager != null)
                {
                    renderPipeline = renderPipelineWorldManager.RenderPipeline;
                }

                // Create the update and render contexts.
                UpdateContext = await _kernel.GetAsync<IUpdateContext>(_node, null, null, new IInjectionAttribute[0], new IConstructorArgument[0], null);
                RenderContext = await _kernel.GetAsync<IRenderContext>(
                    _node, null, null, new IInjectionAttribute[0], new IConstructorArgument[]
                    {
                        new NamedConstructorArgument("renderPipeline", renderPipeline)
                    },
                    null);

                // Configure the render pipeline if possible.
                if (renderPipeline != null)
                {
                    InternalConfigureRenderPipeline(renderPipeline);
                }

                // Retrieve all engine hooks.  These can be set up by additional modules
                // to change runtime behaviour.
                _engineHooks =
                    (await _kernel.GetAllAsync<IEngineHook>(_node, null, null,
                        new IInjectionAttribute[] { new FromGameAttribute() }, new IConstructorArgument[0], null)).ToArray();

                // Now we're ready to enable the main loop and turn off
                // early loading screen rendering.
                _isReadyForMainRenderTakeover = true;

                // Request the game context to load the world.
                GameContext.SwitchWorld<TInitialWorld>();

                // Register with analytics services.
                _analyticsEngine.LogGameplayEvent("Game:Start");
            }
        }

        /// <summary>
        /// Internal use only.  Derive from <see cref="CoreGame{TInitialWorld}"/> and
        /// implement <see cref="CoreGame{TInitialWorld}.ConfigureRenderPipeline"/>
        /// if you want to configure a render pipeline for newer Protogame games.
        /// <para>
        /// This method is not called if the world manager is not <see cref="RenderPipelineWorldManager"/>.
        /// </para>
        /// </summary>
        /// <param name="pipeline">The render pipeline to configure.</param>
        protected virtual void InternalConfigureRenderPipeline(IRenderPipeline pipeline)
        {
            throw new NotSupportedException();
        }
        
        public void Dispose(bool disposing)
        {
            GameContext?.World?.Dispose();

            if (_analyticsEngine != null)
            {
                _analyticsEngine.LogGameplayEvent("Game:Stop");

                _analyticsEngine.FlushAndStop();
            }
        }
        
        public void Update(GameTime gameTime)
        {
            if (!_isReadyForMainRenderTakeover)
            {
                // LoadContent hasn't finished running yet.  At this point, we don't even have
                // the engine hooks loaded, so manually update the coroutine scheduler.
                if (_hasDoneEarlyRender)
                {
                    _coroutineScheduler.Update((IGameContext) null, null);
                }
                return;
            }

            if (_consoleHandle != null && !StartupTrace.EmittedTimingEntries)
            {
                foreach (var kv in StartupTrace.TimingEntries)
                {
                    _consoleHandle.LogDebug("{0}: {1}ms", kv.Key, Math.Round(kv.Value.TotalMilliseconds, 2));
                }

                StartupTrace.EmittedTimingEntries = true;
            }

            using (_profiler.Measure("update", GameContext.FrameCount.ToString()))
            {
                // Measure FPS.
                using (_profiler.Measure("measure_fps"))
                {
                    GameContext.FrameCount += 1;
                    _elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (_elapsedTime >= 1000f)
                    {
                        GameContext.FPS = _totalFrames;
                        _totalFrames = 0;
                        _elapsedTime = 0;
                    }
                }

                // This can be used in case MonoGame does not initialize correctly before the first frame.
                if (GameContext.FrameCount < SkipFrames)
                {
                    return;
                }

                // Update the game.
                using (_profiler.Measure("main"))
                {
                    GameContext.GameTime = gameTime;
                    GameContext.Begin();
                    foreach (var hook in _engineHooks)
                    {
                        hook.Update(GameContext, UpdateContext);
                    }

                    GameContext.WorldManager.Update(this);
                }
            }
        }
        
        public void Draw(GameTime gameTime)
        {
            if (!_isReadyForMainRenderTakeover)
            {
                // LoadContent hasn't finished running yet.  Use the early game loading screen.
                _loadingScreen.RenderEarly(this, _hostGame.SplashScreenSpriteBatch, _hostGame.SplashScreenTexture);
                _hasDoneEarlyRender = true;
                return;
            }

            using (_profiler.Measure("render", (GameContext.FrameCount - 1).ToString()))
            {
                RenderContext.IsRendering = true;

                _totalFrames++;

                // This can be used in case MonoGame does not initialize correctly before the first frame.
                if (GameContext.FrameCount < SkipFrames)
                {
                    _hostGame.GraphicsDevice.Clear(Color.Black);
                    return;
                }

                // Render the game.
                using (_profiler.Measure("main"))
                {
                    GameContext.GameTime = gameTime;
                    if (typeof(TWorldManager) != typeof(RenderPipelineWorldManager))
                    {
                        foreach (var hook in _engineHooks)
                        {
                            hook.Render(GameContext, RenderContext);
                        }
                    }

                    GameContext.WorldManager.Render(this);
                }

#if PLATFORM_ANDROID
                // Recorrect the viewport on Android, which seems to be completely bogus by default.
                this.GameContext.Graphics.GraphicsDevice.Viewport = new Viewport(
                    this.Window.ClientBounds.Left,
                    this.Window.ClientBounds.Top,
                    this.Window.ClientBounds.Width,
                    this.Window.ClientBounds.Height);
#endif

                RenderContext.IsRendering = false;
            }
        }

        /// <summary>
        /// Handles a close requested event, usually occurring when the user clicks the close button
        /// on the game window or presses Alt-F4.
        /// </summary>
        /// <param name="cancel">Whether or not to cancel the form closure.</param>
        public virtual void CloseRequested(out bool cancel)
        {
            cancel = false;
        }

        /// <summary>
        /// Prepares the graphics device manager.
        /// <para>
        /// If you want to change the initial size of the game window on startup, this is the place
        /// to do it.  Override this method, and then set <see cref="Microsoft.Xna.Framework.GraphicsDeviceManager.PreferredBackBufferWidth"/>
        /// and <see cref="Microsoft.Xna.Framework.GraphicsDeviceManager.PreferredBackBufferHeight"/>.
        /// </para>
        /// </summary>
        /// <param name="graphicsDeviceManager">The graphics device manager to prepare.</param>
        public virtual void PrepareGraphicsDeviceManager(GraphicsDeviceManager graphicsDeviceManager)
        {
        }

        /// <summary>
        /// Prepares the game window.
        /// <para>
        /// If you want to change the initial position of the game window on startup, this is the place
        /// to do it.  Override this method, and then set the window properties.
        /// </para>
        /// </summary>
        /// <param name="window">The game window to prepare.</param>
        public virtual void PrepareGameWindow(IGameWindow window)
        {
        }

        /// <summary>
        /// Prepares the graphics devices settings.
        /// </summary>
        /// <remarks>
        /// The default configuration is to enable multisampling.  To disable multisampling,
        /// override PrepareDeviceSettings in your derived class.
        /// </remarks>
        /// <param name="deviceInformation">The device information.</param>
        public virtual void PrepareDeviceSettings(GraphicsDeviceInformation deviceInformation)
		{
            deviceInformation.PresentationParameters.RenderTargetUsage =
                RenderTargetUsage.PreserveContents;

#if PLATFORM_WINDOWS
            // This will select the highest available multisampling.
            deviceInformation.PresentationParameters.MultiSampleCount = 32;
            _hostGame.GraphicsDeviceManager.PreferMultiSampling = true;
#else
            // On non-Windows platforms, MonoGame's support for multisampling is
            // just totally broken.  Even if we ask for it here, the maximum
            // allowable multisampling for the platform won't be detected, which
            // causes render target corruption later on if we try and create
            // render targets with the presentation parameter's multisample
            // count.  This is because on Windows, the property of MultiSampleCount
            // is adjusted down from 32 to whatever multisampling value is actually
            // available, but this does not occur for OpenGL platforms, and so
            // the render targets on OpenGL platforms aren't initialised to a valid
            // state for the GPU to use.
            deviceInformation.PresentationParameters.MultiSampleCount = 0;
            _hostGame.GraphicsDeviceManager.PreferMultiSampling = false;
#endif
        }
    }
}
