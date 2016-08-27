// ReSharper disable CheckNamespace

using Jitter.Dynamics;

namespace Protogame
{
    /// <summary>
    /// An event that signals that two rigid bodies have stopped colliding in the game.
    /// </summary>
    /// <module>Physics</module>
    public class PhysicsCollisionEndEvent : PhysicsEvent
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="PhysicsCollisionEndEvent"/>.  This constructor
        /// is intended to be used internally within the framework.
        /// </summary>
        /// <param name="gameContext">The current game context, or null if running on a server.</param>
        /// <param name="serverContext">The current server context, or null if running on a client.</param>
        /// <param name="updateContext">The current update context.</param>
        /// <param name="body1">The first body involved in the collision.</param>
        /// <param name="body2">The second body involved in the collision.</param>
        /// <param name="owner1">The owner of the first body.</param>
        /// <param name="owner2">The owner of the second body.</param>
        public PhysicsCollisionEndEvent(
            IGameContext gameContext,
            IServerContext serverContext,
            IUpdateContext updateContext,
            RigidBody body1,
            RigidBody body2,
            object owner1,
            object owner2) : base(
                gameContext,
                serverContext,
                updateContext)
        {
            Body1 = body1;
            Body2 = body2;
            Owner1 = owner1;
            Owner2 = owner2;
        }

        /// <summary>
        /// The first body involved in the collision.
        /// </summary>
        public RigidBody Body1 { get; }

        /// <summary>
        /// The second body involved in the collision.
        /// </summary>
        public RigidBody Body2 { get; }

        /// <summary>
        /// The owner of the first body.
        /// </summary>
        public object Owner1 { get; }

        /// <summary>
        /// The owner of the second body.
        /// </summary>
        public object Owner2 { get; }
    }
}
