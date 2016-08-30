using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The server configuration interface.  All of the implementations of
    /// <see cref="IServerConfiguration"/> are instantiated at startup and are
    /// used to configure the dependency injection system and the server.
    /// </summary>
    /// <module>Core API</module>
    public interface IServerConfiguration
    {
        /// <summary>
        /// Called at application startup to configure the kernel
        /// before the server is created.
        /// </summary>
        /// <param name="kernel">The dependency injection kernel.</param>
        void ConfigureKernel(IKernel kernel);

        /// <summary>
        /// Called at application startup to construct the main server
        /// instance.  This instance will be run as the main server.
        /// </summary>
        /// <param name="kernel">The dependency injection kernel.</param>
        /// <returns>The server instance to run.</returns>
        ICoreServer ConstructServer(IKernel kernel);
    }
}
