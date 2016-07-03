using Microsoft.Xna.Framework;

namespace Protogame
{
    using System;
    using Protoinject;

    public class DefaultServerContext : IServerContext
    {
        private readonly IKernel _kernel;

        private IServerWorld _nextWorld;

        public DefaultServerContext(
            IKernel kernel,
            Server server,
            IServerWorld world,
            IServerWorldManager worldManager)
        {
            _kernel = kernel;
            Server = server;
            World = world;
            WorldManager = worldManager;
            
            GameTime = new GameTime();
            Tick = 0;
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
        }

        public IServerWorld World
        {
            get; private set;
        }

        public IServerWorldManager WorldManager
        {
            get;
        }

        public IHierarchy Hierarchy => _kernel.Hierarchy;

        public GameTime GameTime { get; set; }

        /// <summary>
        /// Handles the beginning of a step, switching out the current world for <see cref="_nextWorld"/>
        /// if required.
        /// </summary>
        public void Begin()
        {
            if (_nextWorld != null)
            {
                World?.Dispose();
                World = _nextWorld;

                _nextWorld = null;
            }
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
            return _kernel.Get<T>();
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
            return creator(_kernel.Get<TFactory>());
        }

        /// <summary>
        /// The switch world.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        public void SwitchWorld<T>() where T : IServerWorld
        {
            _nextWorld = CreateWorld<T>();
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
            _nextWorld = CreateWorld(creator);
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
            _nextWorld = world;
        }
    }
}

