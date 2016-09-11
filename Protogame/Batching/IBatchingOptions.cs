// ReSharper disable CheckNamespace

namespace Protogame
{
    /// <summary>
    /// An interface which represents static batching options.
    /// </summary>
    /// <module>Batching</module>
    public interface IBatchingOptions
    {
        /// <summary>
        /// Whether physics components that are marked <see cref="PhysicalBaseRigidBodyComponent.StaticAndImmovable"/> will
        /// be combined into a single rigid body that uses a compound shape.  Once the batching is complete, the physics components
        /// are marked as <see cref="PhysicalBaseRigidBodyComponent.Batched"/>.
        /// </summary>
        bool BatchPhysics { get; }
    }
}