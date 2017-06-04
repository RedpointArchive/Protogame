using System;
using Jitter;
using Jitter.Dynamics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// Represents the physics engine in Protogame.
    /// </summary>
    /// <module>Physics</module>
    public interface IPhysicsEngine
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext);

        void Update(IServerContext serverContext, IUpdateContext updateContext);

        void RegisterRigidBodyForHasMatrixInCurrentWorld(RigidBody rigidBody, IHasTransform hasTransform, bool staticAndImmovable);

        void UnregisterRigidBodyForHasMatrixInCurrentWorld(RigidBody rigidBody, IHasTransform hasTransform);

        void DebugRender(IGameContext gameContext, IRenderContext renderContext);

        JitterWorld GetInternalPhysicsWorld();

        PhysicsMetrics GetPhysicsMetrics();

        /// <summary>
        /// Performs a raycast through the scene using the specified ray and callback.
        /// </summary>
        /// <remarks>
        /// The callback allows you to filter rigidbodies and components out of the raycast as needed.
        /// <para>
        /// This method is not thread-safe and you can not perform raycasts in parallel with this method.
        /// </para>
        /// </remarks>
        /// <param name="ray">The ray to cast along.</param>
        /// <param name="callback">The callback used to select or filter out a given rigidbody.  This can be null.</param>
        /// <param name="rigidBody">The rigidbody that the raycast hit; only a valid value if this method returns true.</param>
        /// <param name="owner">The owner of the rigidbody that the raycast hit; only a valid value if this method returns true.</param>
        /// <param name="normal">The normal of the surface at which the raycast hit; only a valid value if this method returns true.</param>
        /// <param name="distance">The distance along the ray at which the raycast hit; only a valid value if this method returns true.</param>
        /// <returns>True if a rigidbody was hit along the raycast, false otherwise.</returns>
        bool Raycast(Ray ray, PhysicsRaycastCallback callback, ref RigidBody rigidBody, ref object owner, ref Vector3 normal, ref float distance);
    }
}