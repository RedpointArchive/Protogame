using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// This interface is used internally by the engine to map model
    /// vertices to GPU vertices (for vertex buffers).  If you are using
    /// model assets in your game, you should bind at least one implementation
    /// of this interface so that models can be rendered with effects.
    /// </summary>
    public interface IModelRenderConfiguration
    {
        /// <summary>
        /// Returns the mapping function to be used for the given model and effect.  If you don't
        /// know how to map the specified model to the specified effect, return null instead and the
        /// engine will continue querying other implementations of the interface.
        /// <para>
        /// To create a return value from this function, use <see cref="ModelVertexMapping.Create{T}"/>.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The GPU vertex type to map to.</typeparam>
        /// <param name="modelAsset">The model asset that is being mapped.</param>
        /// <param name="effectAsset">The effect asset that the model is being mapped to.</param>
        /// <returns>The model vertex mapping, or <c>null</c> if there is none provided.</returns>
        ModelVertexMapping GetVertexMappingToGPU(Model modelAsset, IEffect effectAsset);
    }
}
