// ReSharper disable CheckNamespace

namespace Protogame
{
    /// <summary>
    /// A default implementation of <see cref="IBatchingOptions"/> that you can use to
    /// configure static batching operations.
    /// </summary>
    /// <module>Batching</module>
    public class BatchingOptions : IBatchingOptions
    {
        /// <summary>
        /// Creates a new set of batching options with the default settings.
        /// </summary>
        public BatchingOptions()
        {
            BatchPhysics = true;
        }

        /// <summary>
        /// Whether physics components that are marked <see cref="PhysicalBaseRigidBodyComponent.StaticAndImmovable"/> will
        /// be combined into a single rigid body that uses a compound shape.  Once the batching is complete, the physics components
        /// are marked as <see cref="PhysicalBaseRigidBodyComponent.Batched"/>.
        /// </summary>
        public bool BatchPhysics { get; set; }
    }
}