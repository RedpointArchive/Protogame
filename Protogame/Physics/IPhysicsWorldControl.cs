using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// This interface provides control over the physics properties of the current world.
    /// </summary>
    /// <module>Physics</module>
    public interface IPhysicsWorldControl
    {
        /// <summary>
        /// The gravity in the current world.
        /// </summary>
        Vector3 Gravity { get; set; }

        /// <summary>
        /// This is an internal method used to synchronise pending changes to the physics world.
        /// The internal physics world state is not always available when users of this interface
        /// set properties, so this method is called by the engine hook to ensure those changes
        /// are propagated to the physics world when it becomes available.
        /// </summary>
        void SyncPendingChanges();
    }
}
