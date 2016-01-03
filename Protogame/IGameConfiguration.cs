using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The game configuration interface.
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
        /// Called at application startup to discover the correct
        /// asset manager provider to use at runtime.
        /// </summary>
        /// <param name="initializer">The asset manager initialiser callback.</param>
        void InitializeAssetManagerProvider(IAssetManagerProviderInitializer initializer);

        /// <summary>
        /// Called at application startup to construct the main game
        /// instance.  This instance will be run as the main game.
        /// </summary>
        /// <param name="kernel">The dependency injection kernel.</param>
        /// <returns>The game instance to run.</returns>
        Game ConstructGame(IKernel kernel);
    }
}
