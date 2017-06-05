// ReSharper disable CheckNamespace

using System.Collections.Generic;
using System.Threading.Tasks;

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
        private readonly ICoroutine _coroutine;

        private IWorld _nextWorld;
        private Task _nextWorldTask;
        
        public DefaultGameContext(
            IKernel kernel,
            IAnalyticsEngine analyticsEngine,
            ICoreGame game, 
            IGameWindow window, 
            IWorld world, 
            IWorldManager worldManager)
        {
            _kernel = kernel;
            _analyticsEngine = analyticsEngine;
            _coroutine = kernel.Get<ICoroutine>();

            Game = game;
            World = world;
            WorldManager = worldManager;
            Window = window;
        }
        
        public int FPS { get; set; }
        
        public int FrameCount { get; set; }
        
        public ICoreGame Game { get; }
        
        public GameTime GameTime { get; set; }
        
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
                if (_nextWorldTask != null && _nextWorldTask.IsCompleted)
                {
                    if (_nextWorldTask.IsFaulted)
                    {
                        throw new AggregateException(_nextWorldTask.Exception);
                    }

                    if (_nextWorldTask.IsCanceled)
                    {
                        throw new OperationCanceledException("The operation to switch the world was cancelled.");
                    }
                }

                World?.Dispose();
                World = _nextWorld;

                _analyticsEngine.LogGameplayEvent("World:Switch:" + World.GetType().Name);

                _nextWorld = null;
            }
        }

        [Obsolete("Use SwitchWorld to asynchronously load worlds.")]
        public IWorld CreateWorld<T>() where T : IWorld
        {
            return _kernel.Get<T>();
        }

        [Obsolete("Use SwitchWorld to asynchronously load worlds.")]
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

            Window.Resize(width, height);
        }
        
        public void SwitchWorld<T>() where T : IWorld
        {
            if (_nextWorldTask != null && _nextWorldTask.Status != TaskStatus.RanToCompletion)
            {
                throw new InvalidOperationException("The game is currently switching to a new world.  You can not call SwitchWorld until it has finished.");
            }

            _nextWorldTask = _coroutine.Run(async () =>
            {
                try
                {
                    _nextWorld = await _kernel.GetAsync<T>((INode)null, (string)null, (string)null, new IInjectionAttribute[0], new IConstructorArgument[0], (Dictionary<Type, List<IMapping>>)null);
                }
                catch (Exception e)
                {
                    throw;
                }
            });
        }

        [Obsolete("Use the other factory-based SwitchWorld where the creator returns Task<IWorld> to asynchronously load worlds.")]
        public void SwitchWorld<TFactory>(Func<TFactory, IWorld> creator)
        {
            _nextWorld = CreateWorld(creator);

            if (_nextWorld == null)
            {
                throw new InvalidOperationException("The world factory returned a null value.");
            }
        }

        public void SwitchWorld<TFactory>(Func<TFactory, Task<IWorld>> creator)
        {
            if (_nextWorldTask != null && _nextWorldTask.Status != TaskStatus.RanToCompletion)
            {
                throw new InvalidOperationException("The game is currently switching to a new world.  You can not call SwitchWorld until it has finished.");
            }

            _nextWorldTask = _coroutine.Run(async () =>
            {
                var factory = await _kernel.GetAsync<TFactory>((INode)null, (string)null, (string)null, new IInjectionAttribute[0], new IConstructorArgument[0], (Dictionary<Type, List<IMapping>>)null);
                _nextWorld = await creator(factory);

                if (_nextWorld == null)
                {
                    throw new InvalidOperationException("The world factory returned a null value.");
                }
            });
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