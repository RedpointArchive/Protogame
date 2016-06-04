using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// Represents a model vertex mapping.  This class encapsulates the configuration in an
    /// immutable class that is easier and safer to construct than the raw values.  Call
    /// the static <see cref="Create{TGPUVertex}"/> method to use it.
    /// </summary>
    public class ModelVertexMapping
    {
        /// <summary>
        /// Creates a model vertex mapping.
        /// </summary>
        /// <typeparam name="TGPUVertex">The type of GPU vertex that this model will be mapped to.</typeparam>
        /// <param name="vertexDeclaration">The vertex declaration, usually the <c>VertexDeclaration</c> property on the original type.</param>
        /// <param name="mapping">The mapping function that maps vertices.</param>
        /// <returns>The model vertex mapping data.</returns>
        public static ModelVertexMapping Create<TGPUVertex>(Func<ModelVertex, TGPUVertex> mapping) where TGPUVertex : IVertexType
        {
            return new ModelVertexMapping(
                i => mapping(i),
                typeof(TGPUVertex));
        }
        
        private ModelVertexMapping(
            Func<ModelVertex, IVertexType> mappingFunction,
            Type vertexType)
        {
            MappingFunction = mappingFunction;
            VertexType = vertexType;

            var temp = mappingFunction(new ModelVertex());
            if (temp == null)
            {
                throw new InvalidOperationException("The GPU mapping function must not return null under any circumstances.");
            }

            VertexDeclaration = temp.VertexDeclaration;
        }

        public Func<ModelVertex, object> MappingFunction { get; } 

        public VertexDeclaration VertexDeclaration { get; }

        public Type VertexType { get; }
    }
}