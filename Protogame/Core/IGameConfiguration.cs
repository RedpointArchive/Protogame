﻿using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The game configuration interface.  All of the implementations of
    /// <see cref="IGameConfiguration"/> are instantiated at startup and are
    /// used to configure the dependency injection system and the game.
    /// </summary>
    /// <module>Core API</module>
    public interface IGameConfiguration
    {
        /// <summary>
        /// Called at application startup to configure the kernel
        /// before the game is created.
        /// </summary>
        /// <param name="kernel">The dependency injection kernel.</param>
        void ConfigureKernel(IKernel kernel);

        /// <summary>
        /// Called at application startup to construct the main game
        /// instance.  This instance will be run as the main game.
        /// </summary>
        /// <param name="kernel">The dependency injection kernel.</param>
        /// <returns>The game instance to run.</returns>
        Game ConstructGame(IKernel kernel);
    }
}
