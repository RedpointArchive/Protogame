namespace Protogame
{
    public interface IServerWorldManager
    {
        /// <summary>
        /// The main update call.  This is invoked by Protogame's <see cref="CoreServer&lt;TInitialWorld, TWorldManager&gt;"/>
        /// implementation.  You should prefer to implement a custom world manager and override update
        /// logic there than override the server's update logic, as the minimum setup and teardown is provided
        /// by the server itself.
        /// </summary>
        /// <param name="server">
        /// The current server instance.
        /// </param>
        /// <typeparam name="T">
        /// The type of the server instance that is running.
        /// </typeparam>
        void Update<T>(T server) where T : Server, ICoreServer;
    }
}

