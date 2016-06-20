namespace Protogame
{
    public interface ICoreServer
    {
        /// <summary>
        /// Gets the server context.
        /// </summary>
        /// <value>
        /// The server context.
        /// </value>
        IServerContext ServerContext { get; }

        /// <summary>
        /// Gets the update context.
        /// </summary>
        /// <value>
        /// The update context.
        /// </value>
        IUpdateContext UpdateContext { get; }

        /// <summary>
        /// Whether the server is currently running.
        /// </summary>
        bool Running { get; }

        /// <summary>
        /// Starts the server in the current thread.
        /// </summary>
        void Run();

        /// <summary>
        /// Stops the server, ending the main loop in the <see cref="Run"/> method.
        /// </summary>
        void Stop();
    }
}

