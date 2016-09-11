// ReSharper disable CheckNamespace

using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The static batching service, which allows you to optimize either a world, an entity or entity
    /// group such that the required number of operations per frame is reduced.
    /// </summary>
    /// <module>Batching</module>
    public interface IStaticBatching
    {
        /// <summary>
        /// Runs the static batcher over the specified node.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="node">The node in the hierarchy to optimize.  You can use <see cref="IHierarchy.Lookup"/> to find the node associated with an object.</param>
        /// <param name="options">The options to use when performing the static batching.</param>
        /// <returns>
        /// The node in the hierarchy that is used to keep the results of the batching updated.
        /// You can remove this node from the hierarchy to remove the effects of batching from the hierarchy.
        /// </returns>
        INode Batch(IGameContext gameContext, INode node, IBatchingOptions options);
    }
}