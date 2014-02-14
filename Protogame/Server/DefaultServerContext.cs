namespace Protogame
{
    using System;
    using Ninject;

    public class DefaultServerContext : IServerContext
    {
        private readonly IKernel m_Kernel;

        public DefaultServerContext(
            IKernel kernel,
            Server server,
            IServerWorld world,
            IServerWorldManager worldManager)
        {
            this.m_Kernel = kernel;
            this.Server = server;
            this.World = world;
            this.WorldManager = worldManager;

            this.Tick = 0;
        }

        public int Tick
        {
            get;
            set;
        }

        public int TimeTick
        {
            get;
            set;
        }

        public DateTime StartTime { get; set; }

        public Server Server
        {
            get;
            internal set;
        }

        public IServerWorld World
        {
            get;
            set;
        }

        public IServerWorldManager WorldManager
        {
            get;
            internal set;
        }

        /// <summary>
        /// The create world.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IWorld"/>.
        /// </returns>
        public IServerWorld CreateWorld<T>() where T : IServerWorld
        {
            return this.m_Kernel.Get<T>();
        }

        /// <summary>
        /// The create world.
        /// </summary>
        /// <param name="creator">
        /// The creator.
        /// </param>
        /// <typeparam name="TFactory">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IWorld"/>.
        /// </returns>
        public IServerWorld CreateWorld<TFactory>(Func<TFactory, IServerWorld> creator)
        {
            return creator(this.m_Kernel.Get<TFactory>());
        }

        /// <summary>
        /// The switch world.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        public void SwitchWorld<T>() where T : IServerWorld
        {
            if (this.World != null)
            {
                this.World.Dispose();
            }

            this.World = this.CreateWorld<T>();
        }

        /// <summary>
        /// The switch world.
        /// </summary>
        /// <param name="creator">
        /// The creator.
        /// </param>
        /// <typeparam name="TFactory">
        /// </typeparam>
        public void SwitchWorld<TFactory>(Func<TFactory, IServerWorld> creator)
        {
            if (this.World != null)
            {
                this.World.Dispose();
            }

            this.World = this.CreateWorld(creator);
        }

        /// <summary>
        /// The switch world.
        /// </summary>
        /// <param name="world">
        /// The world.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public void SwitchWorld<T>(T world) where T : IServerWorld
        {
            if (this.World != null)
            {
                this.World.Dispose();
            }

            this.World = world;
        }
    }
}

