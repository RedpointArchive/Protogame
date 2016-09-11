// ReSharper disable CheckNamespace

using Jitter.Collision.Shapes;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// This factory is used by the batching system to create components and entities
    /// that can be used to represent the result of the batching operations.
    /// </summary>
    /// <module>Batching</module>
    public interface IBatchedControlFactory : IGenerateFactory
    {
        /// <summary>
        /// Creates a <see cref="BatchedControlEntity"/>, which is used to add the results of
        /// a batching operation to the current world or parent object.
        /// </summary>
        /// <returns>The new batched control entity.</returns>
        BatchedControlEntity CreateBatchedControlEntity();

        /// <summary>
        /// Creates a <see cref="PhysicalBatchedStaticCompoundComponent"/> which is used to hold the
        /// static physics rigid body created when <see cref="IBatchingOptions.BatchPhysics"/> is
        /// turned on.
        /// </summary>
        /// <param name="shape">The compound shape to store in the world.</param>
        /// <returns>The new component.</returns>
        PhysicalBatchedStaticCompoundComponent CreatePhysicalBatchedStaticCompoundComponent(CompoundShape shape);
    }
}
