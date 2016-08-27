// ReSharper disable CheckNamespace

using Jitter.Dynamics;

namespace Protogame
{
    /// <summary>
    /// An interface which indicates that this component receives events
    /// about physics collisions.
    /// </summary>
    public interface ICollidableComponent
    {
        /// <summary>
        /// This method is called by the physics system when a collision involving one of this component's
        /// parents and another object with a rigid body starts occurring.
        /// </summary>
        /// <param name="gameContext">The current game context, or null if running on a server.</param>
        /// <param name="serverContext">The current server context, or null if running on a client.</param>
        /// <param name="updateContext">The current update context.</param>
        /// <param name="owner1">The owner of the first rigid body.  This is NOT necessarily one of the component's parents.</param>
        /// <param name="owner2">The owner of the second rigid body.  This is NOT necessarily one of the component's parents.</param>
        /// <param name="body1">The rigid body of the first object.</param>
        /// <param name="body2">The rigid body of the second object.</param>
        void CollisionBegin(IGameContext gameContext, IServerContext serverContext, IUpdateContext updateContext, object owner1, object owner2, RigidBody body1, RigidBody body2);

        /// <summary>
        /// This method is called by the physics system when a collision involving one of this component's
        /// parents and another object with a rigid body finishes (i.e. the rigid bodies have seperated from their collision).
        /// </summary>
        /// <param name="gameContext">The current game context, or null if running on a server.</param>
        /// <param name="serverContext">The current server context, or null if running on a client.</param>
        /// <param name="updateContext">The current update context.</param>
        /// <param name="owner1">The owner of the first rigid body.  This is NOT necessarily one of the component's parents.</param>
        /// <param name="owner2">The owner of the second rigid body.  This is NOT necessarily one of the component's parents.</param>
        /// <param name="body1">The rigid body of the first object.</param>
        /// <param name="body2">The rigid body of the second object.</param>
        void CollisionEnd(IGameContext gameContext, IServerContext serverContext, IUpdateContext updateContext, object owner1, object owner2, RigidBody body1, RigidBody body2);
    }
}
