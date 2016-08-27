// ReSharper disable CheckNamespace

namespace Protogame
{
    /// <summary>
    /// The base, abstract implementation of a physics event.
    /// </summary>
    public abstract class PhysicsEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="PhysicsEvent"/>.  This constructor
        /// is intended to be used internally within the engine.
        /// </summary>
        /// <param name="gameContext">The current game context, or null if running on a server.</param>
        /// <param name="serverContext">The current server context, or null if running on a client.</param>
        /// <param name="updateContext">The current update context.</param>
        protected PhysicsEvent(
            IGameContext gameContext,
            IServerContext serverContext,
            IUpdateContext updateContext)
        {
            GameContext = gameContext;
            ServerContext = serverContext;
            UpdateContext = updateContext;
        }

        /// <summary>
        /// Gets or sets the current game context.  This is null when executing on a server.
        /// </summary>
        public IGameContext GameContext { get; }

        /// <summary>
        /// Gets or sets the current server context.  This is null when executing on a client.
        /// </summary>
        public IServerContext ServerContext { get; }

        /// <summary>
        /// Gets or sets the current update context.
        /// </summary>
        public IUpdateContext UpdateContext { get; }
    }
}
