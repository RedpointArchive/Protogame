namespace Protogame
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Protoinject;

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
        /// <see cref="IRenderPipeline.AddRenderPass"/> to add passes to the render pipeline.
        /// </para>
        /// </summary>
        /// <param name="pipeline">The render pipeline to configure.</param>
        /// <param name="kernel">
        /// The dependency injection kernel, on which you can call 
        /// <see cref="ResolutionExtensions.Get{T}(IResolutionRoot, IParameter[])"/> to create
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
            this.ConfigureRenderPipeline(pipeline, _kernel);
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
    public abstract class CoreGame<TInitialWorld, TWorldManager> : Game, ICoreGame
        where TInitialWorld : IWorld where TWorldManager : IWorldManager
    {
        /// <summary>
        /// The dependency injection kernel.
        /// </summary>
        private readonly IKernel m_Kernel;

        /// <summary>
        /// The graphics device manager instance.
        /// </summary>
        private readonly GraphicsDeviceManager m_GraphicsDeviceManager;

        /// <summary>
        /// The current profiler instance.
        /// </summary>
        private readonly IProfiler m_Profiler;

        /// <summary>
        /// The current analytics engine instance.
        /// </summary>
        private readonly IAnalyticsEngine m_AnalyticsEngine;

        /// <summary>
        /// The current analytics initializer instance.
        /// </summary>
        private readonly IAnalyticsInitializer m_AnalyticsInitializer;

        /// <summary>
        /// The total number of frames that have elapsed since the last second interval.
        /// </summary>
        private int m_TotalFrames;

        /// <summary>
        /// The total amount of time that has elapsed since the last second interval.
        /// </summary>
        private float m_ElapsedTime;

        /// <summary>
        /// An array of the engine hooks that are loaded.
        /// </summary>
        private IEngineHook[] m_EngineHooks;

        /// <summary>
        /// The current hierarchical node of the game in the dependency injection system.
        /// </summary>
        private INode m_Current;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreGame{TInitialWorld,TWorldManager}"/> class. 
        /// </summary>
        /// <param name="kernel">
        /// The dependency injection kernel.
        /// </param>
        protected CoreGame(IKernel kernel)
        {
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

            this.m_Kernel = kernel;
            this.m_Current = this.m_Kernel.CreateEmptyNode("Game");

            this.m_GraphicsDeviceManager = new GraphicsDeviceManager(this);
            PrepareGraphicsDeviceManager(this.m_GraphicsDeviceManager);
            this.m_GraphicsDeviceManager.PreparingDeviceSettings +=
                (sender, e) =>
                {
                    this.PrepareDeviceSettings(e.GraphicsDeviceInformation);
                };

            this.m_Profiler = kernel.TryGet<IProfiler>(this.m_Current);
            if (this.m_Profiler == null)
            {
                kernel.Bind<IProfiler>().To<NullProfiler>();
                this.m_Profiler = kernel.Get<IProfiler>(this.m_Current);
            }

            this.m_AnalyticsEngine = kernel.TryGet<IAnalyticsEngine>(this.m_Current);
            if (this.m_AnalyticsEngine == null)
            {
                kernel.Bind<IAnalyticsEngine>().To<NullAnalyticsEngine>();
                this.m_AnalyticsEngine = kernel.Get<IAnalyticsEngine>(this.m_Current);
            }

            this.m_AnalyticsInitializer = kernel.TryGet<IAnalyticsInitializer>(this.m_Current);
            if (this.m_AnalyticsInitializer == null)
            {
                kernel.Bind<IAnalyticsInitializer>().To<NullAnalyticsInitializer>();
                this.m_AnalyticsInitializer = kernel.Get<IAnalyticsInitializer>(this.m_Current);
            }

            this.m_AnalyticsInitializer.Initialize(this.m_AnalyticsEngine);

            // TODO: Fix this because it means we can't have more than one game using the same IoC container.
            var assetContentManager = new AssetContentManager(this.Services);
            this.Content = assetContentManager;
            kernel.Bind<IAssetContentManager>().ToMethod(x => assetContentManager);
        }

        /// <summary>
        /// A platform independent representation of a game window.
        /// </summary>
        /// <value>
        /// The game window.
        /// </value>
        public new IGameWindow Window { get; private set; }

        /// <summary>
        /// The graphics device manager used by the game.
        /// </summary>
        /// <value>
        /// The graphics device manager.
        /// </value>
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                return this.m_GraphicsDeviceManager;
            }
        }

        /// <summary>
        /// The number of frames to skip before updating or rendering.
        /// </summary>
        public int SkipFrames { get; set; }

        /// <summary>
        /// The load content.
        /// </summary>
        protected override void LoadContent()
        {
            // The interception library can't properly intercept class types, which
            // means we can't simply do this.m_Kernel.Get<TInitialWorld>() because
            // none of the calls will be intercepted.  Instead, we need to bind the
            // IWorld and IWorldManager to their initial types and then unbind them
            // after they've been constructed.
            this.m_Kernel.Bind<IWorld>().To<TInitialWorld>();
            this.m_Kernel.Bind<IWorldManager>().To<TWorldManager>();
            var world = this.m_Kernel.Get<IWorld>(this.m_Current);
            var worldManager = this.m_Kernel.Get<IWorldManager>(this.m_Current);
            this.m_Kernel.Unbind<IWorld>();
            this.m_Kernel.Unbind<IWorldManager>();

            // Construct a platform-independent game window.
            this.Window = this.ConstructGameWindow();

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
            base.Window.ClientSizeChanged += (sender, e) =>
            {
                if (!shouldHandleResize)
                {
                    return;
                }

                shouldHandleResize = false;
                var width = base.Window.ClientBounds.Width;
                var height = base.Window.ClientBounds.Height;
                this.GameContext.Graphics.PreferredBackBufferWidth = width;
                this.GameContext.Graphics.PreferredBackBufferHeight = height;
                this.GameContext.Graphics.ApplyChanges();
                shouldHandleResize = true;
            };

            // Register for the window close event so we can dispatch
            // it correctly.
            var form = System.Windows.Forms.Control.FromHandle(base.Window.Handle) as System.Windows.Forms.Form;
            if (form != null)
            {
                form.FormClosing += (sender, args) =>
                {
                    bool cancel;
                    this.CloseRequested(out cancel);

                    if (cancel)
                    {
                        args.Cancel = true;
                    }
                };
            }
#endif

            // Create the game context.
            this.GameContext = this.m_Kernel.Get<IGameContext>(
                this.m_Current,
                new NamedConstructorArgument("game", this), 
                new NamedConstructorArgument("graphics", this.m_GraphicsDeviceManager), 
                new NamedConstructorArgument("world", world), 
                new NamedConstructorArgument("worldManager", worldManager), 
                new NamedConstructorArgument("window", this.ConstructGameWindow()));

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
            this.UpdateContext = this.m_Kernel.Get<IUpdateContext>(this.m_Current);
            this.RenderContext = this.m_Kernel.Get<IRenderContext>(
                this.m_Current,
                new NamedConstructorArgument("renderPipeline", renderPipeline));

            // Configure the render pipeline if possible.
            if (renderPipeline != null)
            {
                this.InternalConfigureRenderPipeline(renderPipeline);
            }

            // Retrieve all engine hooks.  These can be set up by additional modules
            // to change runtime behaviour.
            this.m_EngineHooks =
                this.m_Kernel.GetAll<IEngineHook>(this.m_Current, null, null,
                    new IInjectionAttribute[] {new FromGameAttribute()}).ToArray();

            // Register with analytics services.
            this.m_AnalyticsEngine.LogGameplayEvent("Game:Start");
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

        /// <summary>
        /// Cleans up and disposes resources used by the game.
        /// </summary>
        /// <param name="disposing">No documentation.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.GameContext != null && this.GameContext.World != null)
            {
                this.GameContext.World.Dispose();
            }

            if (this.m_AnalyticsEngine != null)
            {
                this.m_AnalyticsEngine.LogGameplayEvent("Game:Stop");

                this.m_AnalyticsEngine.FlushAndStop();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
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

                // This can be used in case MonoGame does not initialize correctly before the first frame.
                if (this.GameContext.FrameCount < this.SkipFrames)
                {
                    return;
                }

                // Update the game.
                using (this.m_Profiler.Measure("main"))
                {
                    this.GameContext.GameTime = gameTime;
                    this.GameContext.Begin();
                    foreach (var hook in this.m_EngineHooks)
                    {
                        hook.Update(this.GameContext, this.UpdateContext);
                    }

                    this.GameContext.WorldManager.Update(this);
                }

                base.Update(gameTime);
            }
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        protected override void Draw(GameTime gameTime)
        {
            using (this.m_Profiler.Measure("render", (this.GameContext.FrameCount - 1).ToString()))
            {
                RenderContext.IsRendering = true;

                this.m_TotalFrames++;

                // This can be used in case MonoGame does not initialize correctly before the first frame.
                if (this.GameContext.FrameCount < this.SkipFrames)
                {
                    this.GraphicsDevice.Clear(Color.Black);
                    return;
                }

                // Render the game.
                using (this.m_Profiler.Measure("main"))
                {
                    this.GameContext.GameTime = gameTime;
                    if (typeof(TWorldManager) != typeof(RenderPipelineWorldManager))
                    {
                        foreach (var hook in this.m_EngineHooks)
                        {
                            hook.Render(this.GameContext, this.RenderContext);
                        }
                    }

                    this.GameContext.WorldManager.Render(this);
                }

                base.Draw(gameTime);

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
        protected virtual void CloseRequested(out bool cancel)
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
        protected virtual void PrepareGraphicsDeviceManager(GraphicsDeviceManager graphicsDeviceManager)
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
        protected virtual void PrepareDeviceSettings(GraphicsDeviceInformation deviceInformation)
		{
            deviceInformation.PresentationParameters.RenderTargetUsage =
                RenderTargetUsage.PreserveContents;

            // This will select the highest available multisampling.
            deviceInformation.PresentationParameters.MultiSampleCount = 32;

            // On OpenGL platform, we need to set this to true otherwise it
            // won't use multisampling regardless of whether we configure it.
            this.m_GraphicsDeviceManager.PreferMultiSampling = true;
        }

        /// <summary>
        /// Constructs an implementation of <see cref="IGameWindow"/> based on the current game.  This method
        /// abstracts the current platform.
        /// </summary>
        /// <returns>
        /// The game window instance.
        /// </returns>
        private IGameWindow ConstructGameWindow()
        {
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX || PLATFORM_WEB || PLATFORM_IOS
            return new DefaultGameWindow(base.Window);
#elif PLATFORM_ANDROID || PLATFORM_OUYA
            return new AndroidGameWindow((Microsoft.Xna.Framework.AndroidGameWindow)base.Window);
#endif
        }

#if PLATFORM_ANDROID || PLATFORM_OUYA
        public Android.Views.View AndroidGameView
        {
            get
            {
                return (Android.Views.View)this.Services.GetService(typeof(Android.Views.View));
            }
        }
#endif

        /// <summary>
        /// Runs code before MonoGame performs any initialization logic.
        /// </summary>
        static CoreGame()
        {
#if PLATFORM_LINUX
            LoadPrimusRunPathForDualGPUDevices();
#endif
        }

#if PLATFORM_LINUX
        public static void LoadPrimusRunPathForDualGPUDevices()
        {
            const string primusRunPath = "/usr/bin/primusrun";
            var basePath = new System.IO.FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).DirectoryName;
            if (System.IO.File.Exists(primusRunPath))
            {
                // primusrun exists, we should try and upgrade
                // the graphics before libGL is loaded so that we
                // can use the NVIDIA GPU instead of Intel (which
                // generally doesn't work with the render pipeline
                // on Linux).
                Console.Error.WriteLine(
                    "Detected Linux system with primusrun; will attempt to use NVIDIA GPU!");
                var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = primusRunPath,
                    Arguments = "env",
                    WorkingDirectory = basePath
                };
                process.Start();
                using (var output = process.StandardOutput)
                {
                    var regex = new System.Text.RegularExpressions.Regex(
                        "^LD_LIBRARY_PATH=(.*)$",
                        System.Text.RegularExpressions.RegexOptions.Multiline);
                    var m = regex.Match(output.ReadToEnd());
                    if (m.Success)
                    {
                        var ldLibraryPath = m.Groups[1].Value;

                        Console.Error.WriteLine("Creating symbolic links to NVIDIA libGL...");
                        var created = new System.Collections.Generic.List<string>();
                        foreach (var path in ldLibraryPath.Split(':'))
                        {
                            var dir = new System.IO.DirectoryInfo(path);
                            if (dir.Exists)
                            {
                                foreach (var f in dir.GetFiles())
                                {
                                    if (!created.Contains(f.Name) && !System.IO.File.Exists(System.IO.Path.Combine(basePath, f.Name)))
                                    {
                                        Console.Error.WriteLine("Mapping " + f.Name + " to " + f.FullName + "...");
                                        var ln = new System.Diagnostics.Process();
                                        ln.StartInfo = new System.Diagnostics.ProcessStartInfo
                                        {
                                            UseShellExecute = false,
                                            FileName = "/usr/bin/ln",
                                            Arguments = "-s '" + f.FullName + "' '" + System.IO.Path.Combine(basePath, f.Name) + "'",
                                            WorkingDirectory = basePath
                                        };
                                        ln.Start();
                                        ln.WaitForExit();
                                        created.Add(f.Name);
                                    }
                                }
                            }
                        }
                        Console.Error.WriteLine("Created symbolic links so that NVIDIA GPU is used.");
                    }
                    else
                    {
                        Console.Error.WriteLine(
                            "Unable to find newer LD_LIBRARY_PATH, rendering might not work correctly!");
                    }
                }
            }
        }
#endif
    }
}
