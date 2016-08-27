// ReSharper disable CheckNamespace
#pragma warning disable 1591

namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    using Protoinject;

    /// <summary>
    /// The default implementation of <see cref="IGameContext"/>.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IGameContext</interface_ref>
    internal class DefaultGameContext : IGameContext
    {
        private readonly IKernel _kernel;
        private readonly IAnalyticsEngine _analyticsEngine;

        private IWorld _nextWorld;
        
        public DefaultGameContext(
            IKernel kernel,
            IAnalyticsEngine analyticsEngine,
            Game game, 
            GraphicsDeviceManager graphics, 
            IGameWindow window, 
            IWorld world, 
            IWorldManager worldManager)
        {
            _kernel = kernel;
            _analyticsEngine = analyticsEngine;
            Game = game;
            Graphics = graphics;
            World = world;
            WorldManager = worldManager;
            Window = window;
        }
        
        public int FPS { get; set; }
        
        public int FrameCount { get; set; }
        
        public Game Game { get; }
        
        public GameTime GameTime { get; set; }
        
        public GraphicsDeviceManager Graphics { get; }
        
        public IGameWindow Window { get; }
        
        public IWorld World { get; private set; }
        
        public IWorldManager WorldManager { get; }
        
        public IHierarchy Hierarchy => _kernel.Hierarchy;
        
        public Ray MouseRay { get; set; }
        
        public Plane MouseHorizontalPlane { get; set; }
        
        public Plane MouseVerticalPlane { get; set; }
        
        public void Begin()
        {
            if (_nextWorld != null)
            {
                World?.Dispose();
                World = _nextWorld;

                _analyticsEngine.LogGameplayEvent("World:Switch:" + World.GetType().Name);

                _nextWorld = null;
            }
        }
        
        public IWorld CreateWorld<T>() where T : IWorld
        {
            return _kernel.Get<T>();
        }
        
        public IWorld CreateWorld<TFactory>(Func<TFactory, IWorld> creator)
        {
            return creator(_kernel.Get<TFactory>());
        }
        
        public void ResizeWindow(int width, int height)
        {
            var coreGame = Game as ICoreGame;
            if (coreGame != null && coreGame.RenderContext.IsRendering)
            {
                throw new InvalidOperationException(
                    "You can not resize the game window while rendering.  You should move " +
                    "the ResizeWindow call into the update loop instead.");
            }

            if (Window.ClientBounds.Width == width && Window.ClientBounds.Height == height)
            {
                return;
            }

            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.ApplyChanges();
        }
        
        public void SwitchWorld<T>() where T : IWorld
        {
            _nextWorld = CreateWorld<T>();
        }
        
        public void SwitchWorld<TFactory>(Func<TFactory, IWorld> creator)
        {
            _nextWorld = CreateWorld(creator);

            if (_nextWorld == null)
            {
                throw new InvalidOperationException("The world factory returned a null value.");
            }
        }
        
        public void SwitchWorld<T>(T world) where T : IWorld
        {
            if (world == null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            _nextWorld = world;
        }
    }
}