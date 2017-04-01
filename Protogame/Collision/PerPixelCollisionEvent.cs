namespace Protogame
{
    /// <summary>
    /// An event that represents a per-pixel 2D collision occurring.
    /// </summary>
    /// <module>Collision</module>
    public class PerPixelCollisionEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="PerPixelCollisionEvent"/>.  This constructor
        /// is intended to be used internally within the engine.
        /// </summary>
        /// <param name="gameContext">The current game context, or null if running on a server.</param>
        /// <param name="serverContext">The current server context, or null if running on a client.</param>
        /// <param name="updateContext">The current update context.</param>
        /// <param name="obj1">The first object involved in the collision.</param>
        /// <param name="obj2">The second object involved in the collision.</param>
        public PerPixelCollisionEvent(
            IGameContext gameContext, 
            IServerContext serverContext, 
            IUpdateContext updateContext,
            object obj1,
            object obj2)
        {
            GameContext = gameContext;
            ServerContext = serverContext;
            UpdateContext = updateContext;
            Object1 = obj1;
            Object2 = obj2;
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

        /// <summary>
        /// The first object involved in the collision.
        /// </summary>
        public object Object1 { get; }

        /// <summary>
        /// The second object involved in the collision.
        /// </summary>
        public object Object2 { get; }
    }
}
