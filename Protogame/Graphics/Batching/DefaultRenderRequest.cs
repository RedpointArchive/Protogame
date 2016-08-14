using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultRenderRequest : IRenderRequest
    {
        private readonly Action<List<Matrix>, VertexBuffer, IndexBuffer> _computeCombinedBuffers;

        public DefaultRenderRequest(
            IRenderContext renderContext,
            RasterizerState rasterizerState,
            BlendState blendState,
            DepthStencilState depthStencilState,
            IEffect effect,
            string techniqueName, 
            IEffectParameterSet effectParameterSet,
            VertexBuffer meshVertexBuffer,
            IndexBuffer meshIndexBuffer,
            PrimitiveType primitiveType,
            Matrix[] instances, 
            Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers)
        {
#if DEBUG
            GraphicsMetricsProfilerVisualiser.RenderRequestsCreated++;
#endif

            RasterizerState = rasterizerState;
            BlendState = blendState;
            DepthStencilState = depthStencilState;
            Effect = effect;
            TechniqueName = techniqueName;
            EffectParameterSet = effectParameterSet;
            MeshVertexBuffer = meshVertexBuffer;
            MeshIndexBuffer = meshIndexBuffer;
            PrimitiveType = primitiveType;
            Instances = instances;
            _computeCombinedBuffers = computeCombinedBuffers;

            // Now that the parameter set has been used in a request, prevent it
            // from being changed.
            EffectParameterSet.Lock(renderContext);

            StateHash =
                RasterizerState.GetHashCode() ^ 397 +
                BlendState.GetHashCode() ^ 397 +
                DepthStencilState.GetHashCode() ^ 397 +
                Effect.GetHashCode() ^ 397 +
                TechniqueName.GetHashCode() ^ 397 +
                EffectParameterSet.GetStateHash() ^ 397 +
                MeshVertexBuffer.GetHashCode() ^ 397 +
                MeshIndexBuffer.GetHashCode() ^ 397 +
                PrimitiveType.GetHashCode() ^ 397;
        }

        public int StateHash { get; }

        public RasterizerState RasterizerState { get; }

        public BlendState BlendState { get; }

        public DepthStencilState DepthStencilState { get; }

        public IEffect Effect { get; }

        public string TechniqueName { get; }

        public IEffectParameterSet EffectParameterSet { get; }

        public VertexBuffer MeshVertexBuffer { get; set; }

        public IndexBuffer MeshIndexBuffer { get; set; }

        public PrimitiveType PrimitiveType { get; set; }

        public Matrix[] Instances { get; }

        public void ComputeInstancesToCustomBuffers(List<Matrix> matrices, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            _computeCombinedBuffers(matrices, vertexBuffer, indexBuffer);
        }

        public bool SupportsComputingInstancesToCustomBuffers => _computeCombinedBuffers != null;
    }
}