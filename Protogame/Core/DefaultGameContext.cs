namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    using Protoinject;

    /// <summary>
    /// The default game context.
    /// </summary>
    internal class DefaultGameContext : IGameContext
    {
        private readonly IKernel _kernel;
        private readonly IAnalyticsEngine _analyticsEngine;

        private IWorld _nextWorld;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultGameContext"/> class.
        /// </summary>
        /// <param name="kernel">
        /// The kernel.
        /// </param>
        /// <param name="game">
        /// The game.
        /// </param>
        /// <param name="graphics">
        /// The graphics.
        /// </param>
        /// <param name="window">
        /// The window.
        /// </param>
        /// <param name="world">
        /// The world.
        /// </param>
        /// <param name="worldManager">
        /// The world manager.
        /// </param>
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

        /// <summary>
        /// Gets or sets the fps.
        /// </summary>
        /// <value>
        /// The fps.
        /// </value>
        public int FPS { get; set; }

        /// <summary>
        /// Gets or sets the frame count.
        /// </summary>
        /// <value>
        /// The frame count.
        /// </value>
        public int FrameCount { get; set; }

        /// <summary>
        /// Gets the game.
        /// </summary>
        /// <value>
        /// The game.
        /// </value>
        public Game Game { get; }

        /// <summary>
        /// Gets or sets the game time.
        /// </summary>
        /// <value>
        /// The game time.
        /// </value>
        public GameTime GameTime { get; set; }

        /// <summary>
        /// Gets or sets the graphics.
        /// </summary>
        /// <value>
        /// The graphics.
        /// </value>
        public GraphicsDeviceManager Graphics { get; }

        /// <summary>
        /// Gets or sets the window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        public IGameWindow Window { get; }

        /// <summary>
        /// Gets or sets the world.
        /// </summary>
        /// <value>
        /// The world.
        /// </value>
        public IWorld World { get; private set; }

        /// <summary>
        /// Gets the world manager.
        /// </summary>
        /// <value>
        /// The world manager.
        /// </value>
        public IWorldManager WorldManager { get; }

        /// <summary>
        /// Gets the dependency injection hierarchy, which contains all worlds, entities and components.
        /// </summary>
        /// <value>
        /// The dependency injection hierarchy, which contains all worlds, entities and components.
        /// </value>
        public IHierarchy Hierarchy => _kernel.Hierarchy;

        /// <summary>
        /// Gets or sets the ray representing the mouse cursor in 3D space.  This is
        /// updated automatically by DefaultRenderContext based on the World, View and Projection
        /// properties of the current render context.
        /// </summary>
        /// <value>The ray representing the mouse cursor in 3D space.</value>
        public Ray MouseRay { get; set; }

        /// <summary>
        /// Gets or sets the plane representing the mouse cursor's Y position in 3D space.  This forms
        /// a plane such that if it were projected back to the screen it would intersect the mouse's
        /// Y position along the X axis of the screen.  This is updated automatically by 
        /// DefaultRenderContext based on the World, View and Projection properties of the current render context.
        /// </summary>
        /// <value>The plane representing the mouse cursor's Y position in 3D space.</value>
        public Plane MouseHorizontalPlane { get; set; }

        /// <summary>
        /// Gets or sets the plane representing the mouse cursor's X position in 3D space.  This forms
        /// a plane such that if it were projected back to the screen it would intersect the mouse's
        /// X position along the Y axis of the screen.  This is updated automatically by 
        /// DefaultRenderContext based on the World, View and Projection properties of the current render context.
        /// </summary>
        /// <value>The plane representing the mouse cursor's X position in 3D space.</value>
        public Plane MouseVerticalPlane { get; set; }

        /// <summary>
        /// Handles the beginning of a frame, switching out the current world for <see cref="_nextWorld"/>
        /// if required.
        /// </summary>
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

        /// <summary>
        /// The create world.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IWorld"/>.
        /// </returns>
        public IWorld CreateWorld<T>() where T : IWorld
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
        public IWorld CreateWorld<TFactory>(Func<TFactory, IWorld> creator)
        {
            return creator(_kernel.Get<TFactory>());
        }

        /// <summary>
        /// The resize window.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
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

        /// <summary>
        /// The switch world.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        public void SwitchWorld<T>() where T : IWorld
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
        public void SwitchWorld<TFactory>(Func<TFactory, IWorld> creator)
        {
            _nextWorld = CreateWorld(creator);

            if (_nextWorld == null)
            {
                throw new InvalidOperationException("The world factory returned a null value.");
            }
        }

        /// <summary>
        /// The switch world.
        /// </summary>
        /// <param name="world">
        /// The world.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
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