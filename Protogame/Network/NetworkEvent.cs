namespace Protogame
{
    public class NetworkEvent : Event
    {
        /// <summary>
        /// Gets or sets the current game context.  This is null when executing on a server.
        /// </summary>
        public IGameContext GameContext { get; set; }

        /// <summary>
        /// Gets or sets the current server context.  This is null when executing on a client.
        /// </summary>
        public IServerContext ServerContext { get; set; }

        /// <summary>
        /// Gets or sets the current update context.
        /// </summary>
        public IUpdateContext UpdateContext { get; set; }

        /// <summary>
        /// Gets or sets the dispatcher.
        /// </summary>
        /// <value>
        /// The dispatcher.
        /// </value>
        public MxDispatcher Dispatcher { get; set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public MxClient Client { get; set; }
    }
}
