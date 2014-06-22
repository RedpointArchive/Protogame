namespace Protogame
{
    using System;
    using Ninject;
    using Ninject.Parameters;

    public class CoreServer<TInitialServerWorld, TServerWorldManager> : Server, ICoreServer, IDisposable
        where TInitialServerWorld : IServerWorld where TServerWorldManager : IServerWorldManager
    {
        private readonly IKernel m_Kernel;

        private readonly IProfiler m_Profiler;

        /// <summary>
        /// The current analytics engine instance.
        /// </summary>
        private readonly IAnalyticsEngine m_AnalyticsEngine;

        /// <summary>
        /// The current analytics initializer instance.
        /// </summary>
        private readonly IAnalyticsInitializer m_AnalyticsInitializer;

        private readonly ITickRegulator m_TickRegulator;

        private bool m_HasSetup;

        private bool m_Stopping;

        public CoreServer(IKernel kernel)
        {
            this.m_Kernel = kernel;

            this.m_Profiler = kernel.TryGet<IProfiler>();
            if (this.m_Profiler == null)
            {
                kernel.Bind<IProfiler>().To<NullProfiler>();
                this.m_Profiler = kernel.Get<IProfiler>();
            }

            this.m_AnalyticsEngine = kernel.TryGet<IAnalyticsEngine>();
            if (this.m_Profiler == null)
            {
                kernel.Bind<IAnalyticsEngine>().To<NullAnalyticsEngine>();
                this.m_AnalyticsEngine = kernel.Get<IAnalyticsEngine>();
            }

            this.m_AnalyticsInitializer = kernel.TryGet<IAnalyticsInitializer>();
            if (this.m_Profiler == null)
            {
                kernel.Bind<IAnalyticsInitializer>().To<NullAnalyticsInitializer>();
                this.m_AnalyticsInitializer = kernel.Get<IAnalyticsInitializer>();
            }

            this.m_AnalyticsInitializer.Initialize(this.m_AnalyticsEngine);

            this.m_TickRegulator = this.m_Kernel.Get<ITickRegulator>();
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
                this.ServerContext.TimeTick = (int)(DateTime.Now - this.ServerContext.StartTime).TotalMilliseconds;

                this.ServerContext.WorldManager.Update(this);

                this.ServerContext.Tick++;
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
            var world = this.m_Kernel.Get<IServerWorld>();
            var worldManager = this.m_Kernel.Get<IServerWorldManager>();
            this.m_Kernel.Unbind<IServerWorld>();
            this.m_Kernel.Unbind<IServerWorldManager>();

            // Create the game context.
            this.ServerContext = this.m_Kernel.Get<IServerContext>(
                new ConstructorArgument("server", this), 
                new ConstructorArgument("world", world), 
                new ConstructorArgument("worldManager", worldManager));
            this.ServerContext.StartTime = DateTime.Now;

            // Create the update context.
            this.UpdateContext = this.m_Kernel.Get<IUpdateContext>();
        }
    }
}

