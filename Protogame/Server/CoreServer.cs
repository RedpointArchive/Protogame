using System.Diagnostics;
using System.Linq;

namespace Protogame
{
    using System;
    using Protoinject;

    /// <summary>
    /// The core Protogame server implementation.  You should derive your server class from this
    /// implementation.
    /// </summary>
    /// <typeparam name="TInitialServerWorld">The initial server world class to start the server with.</typeparam>
    /// <module>Server</module>
    public class CoreServer<TInitialServerWorld> : CoreServer<TInitialServerWorld, DefaultServerWorldManager>,
        ICoreServer, IDisposable
        where TInitialServerWorld : IServerWorld
    {
        public CoreServer(IKernel kernel) : base(kernel)
        {
        }
    }

    /// <summary>
    /// The implementation of Protogame's base server class.  In previous versions of Protogame, this
    /// acted as the base class for the developer's Game class.  However newer games written in
    /// Protogame should use <see cref="CoreServer{TInitialWorld}"/> instead, as this correctly
    /// sets games to use the default server world manager, which sets TWorldManager to be
    /// <see cref="DefaultServerWorldManager"/> automatically.
    /// </summary>
    /// <typeparam name="TInitialServerWorld">The initial server world class to start the server with.</typeparam>
    /// <typeparam name="TServerWorldManager">The server world manager class for this game.</typeparam>
    /// <module>Server</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.CoreServer{TInitialServerWorld}</interface_ref>
    public class CoreServer<TInitialServerWorld, TServerWorldManager> : Server, ICoreServer, IDisposable
        where TInitialServerWorld : IServerWorld where TServerWorldManager : IServerWorldManager
    {
        private readonly IKernel m_Kernel;

        private readonly IProfiler m_Profiler;

        private readonly INode _current;

        /// <summary>
        /// The current analytics engine instance.
        /// </summary>
        private readonly IAnalyticsEngine m_AnalyticsEngine;

        /// <summary>
        /// The current analytics initializer instance.
        /// </summary>
        private readonly IAnalyticsInitializer m_AnalyticsInitializer;

        private readonly ITickRegulator m_TickRegulator;

        /// <summary>
        /// An array of the engine hooks that are loaded.
        /// </summary>
        private IEngineHook[] _engineHooks;

        private bool m_HasSetup;

        private bool m_Stopping;

        private TimeSpan _targetElapsedTime = TimeSpan.FromTicks(166667); // 60fps
        private TimeSpan _inactiveSleepTime = TimeSpan.FromSeconds(0.02);

        private TimeSpan _maxElapsedTime = TimeSpan.FromMilliseconds(500);

        private TimeSpan _accumulatedElapsedTime;

        private Stopwatch _gameTimer;

        private long _previousTicks = 0;

        public CoreServer(IKernel kernel)
        {
            this.m_Kernel = kernel;
            _current = this.m_Kernel.CreateEmptyNode("Server");

            this.m_Profiler = kernel.TryGet<IProfiler>(_current);
            if (this.m_Profiler == null)
            {
                kernel.Bind<IProfiler>().To<NullProfiler>();
                this.m_Profiler = kernel.Get<IProfiler>(_current);
            }

            this.m_AnalyticsEngine = kernel.TryGet<IAnalyticsEngine>(_current);
            if (this.m_AnalyticsEngine == null)
            {
                kernel.Bind<IAnalyticsEngine>().To<NullAnalyticsEngine>();
                this.m_AnalyticsEngine = kernel.Get<IAnalyticsEngine>(_current);
            }

            this.m_AnalyticsInitializer = kernel.TryGet<IAnalyticsInitializer>(_current);
            if (this.m_AnalyticsInitializer == null)
            {
                kernel.Bind<IAnalyticsInitializer>().To<NullAnalyticsInitializer>();
                this.m_AnalyticsInitializer = kernel.Get<IAnalyticsInitializer>(_current);
            }

            this.m_AnalyticsInitializer.Initialize(this.m_AnalyticsEngine);

            this.m_TickRegulator = this.m_Kernel.Get<ITickRegulator>(_current);

            _gameTimer = Stopwatch.StartNew();
        }

        public IServerContext ServerContext
        {
            get;
            private set;
        }

        public IUpdateContext UpdateContext
        {
            get;
            private set;
        }

        public bool Running
        {
            get
            {
                return !this.m_Stopping;
            }
        }

        public void Dispose()
        {
            this.ServerContext.World.Dispose();
            
            this.m_AnalyticsEngine.LogGameplayEvent("Server:Stop");

            this.m_AnalyticsEngine.FlushAndStop();
        }

        protected virtual void LoadContent()
        {
        }

        protected virtual void Update()
        {
            using (this.m_Profiler.Measure("tick"))
            {
                var currentTicks = _gameTimer.Elapsed.Ticks;
                _accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
                _previousTicks = currentTicks;

                if (_accumulatedElapsedTime > _maxElapsedTime)
                    _accumulatedElapsedTime = _maxElapsedTime;

                this.ServerContext.GameTime.ElapsedGameTime = _targetElapsedTime;

                // Perform as many full fixed length time steps as we can.
                while (_accumulatedElapsedTime >= _targetElapsedTime)
                {
                    this.ServerContext.GameTime.TotalGameTime += _targetElapsedTime;
                    _accumulatedElapsedTime -= _targetElapsedTime;

                    this.ServerContext.TimeTick = (int)(DateTime.Now - this.ServerContext.StartTime).TotalMilliseconds;

                    this.ServerContext.Begin();
                    this.ServerContext.WorldManager.Update(this);

                    this.ServerContext.Tick++;
                }
            }
        }

        public void Stop()
        {
            this.m_Stopping = true;
        }

        public void Run()
        {
            if (!this.m_HasSetup)
            {
                this.Setup();
            }

            this.LoadContent();

            this.m_AnalyticsEngine.LogGameplayEvent("Server:Start");

            while (!this.m_Stopping)
            {
                this.m_TickRegulator.WaitUntilReady();

                foreach (var hook in _engineHooks)
                {
                    hook.Update(this.ServerContext, this.UpdateContext);
                }

                this.Update();
            }
        }

        public void Setup()
        {
            if (this.m_HasSetup)
            {
                throw new InvalidOperationException(
                    "The server has already been setup.");
            }

            this.m_HasSetup = true;

            // The interception library can't properly intercept class types, which
            // means we can't simply do this.m_Kernel.Get<TInitialServerWorld>() because
            // none of the calls will be intercepted.  Instead, we need to bind the
            // IServerWorld and IServerWorldManager to their initial types and then unbind them
            // after they've been constructed.
            this.m_Kernel.Bind<IServerWorld>().To<TInitialServerWorld>();
            this.m_Kernel.Bind<IServerWorldManager>().To<TServerWorldManager>();
            var world = this.m_Kernel.Get<IServerWorld>(_current);
            var worldManager = this.m_Kernel.Get<IServerWorldManager>(_current);
            this.m_Kernel.Unbind<IServerWorld>();
            this.m_Kernel.Unbind<IServerWorldManager>();

            // Create the game context.
            this.ServerContext = this.m_Kernel.Get<IServerContext>(
                _current,
                new NamedConstructorArgument("server", this), 
                new NamedConstructorArgument("world", world), 
                new NamedConstructorArgument("worldManager", worldManager));
            this.ServerContext.StartTime = DateTime.Now;

            // Create the update context.
            this.UpdateContext = this.m_Kernel.Get<IUpdateContext>(_current);

            // Retrieve all engine hooks.  These can be set up by additional modules
            // to change runtime behaviour.
            _engineHooks =
                this.m_Kernel.GetAll<IEngineHook>(_current, null, null,
                    new IInjectionAttribute[] { new FromServerAttribute() }).ToArray();
        }
    }
}

