using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IRenderBatcher
    {
        ulong LastBatchCount { get; }

        ulong LastApplyCount { get; }

        ulong LastBatchSaveCount { get; }

        void QueueRequest(IRenderContext renderContext, IRenderRequest request);

        void FlushRequests(IGameContext gameContext, IRenderContext renderContext);

        void RenderRequestImmediate(IRenderContext renderContext, IRenderRequest request);

        IRenderRequest CreateSingleRequest(
            IRenderContext renderContext,
            RasterizerState rasterizerState, 
            BlendState blendState,
            DepthStencilState depthStencilState,
            IEffect effect, 
            IEffectParameterSet effectParameterSet, 
            VertexBuffer meshVertexBuffer,
            IndexBuffer meshIndexBuffer, 
            PrimitiveType primitiveType,
            Matrix world, 
            Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers);

        IRenderRequest CreateSingleRequestFromState(
            IRenderContext renderContext, 
            IEffect effect, 
            IEffectParameterSet effectParameterSet, 
            VertexBuffer meshVertexBuffer, 
            IndexBuffer meshIndexBuffer,
            PrimitiveType primitiveType,
            Matrix world, 
            Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers);

        IRenderRequest CreateInstancedRequest(
            IRenderContext renderContext,
            RasterizerState rasterizerState,
            BlendState blendState,
            DepthStencilState depthStencilState,
            IEffect effect,
            IEffectParameterSet effectParameterSet,
            VertexBuffer meshVertexBuffer,
            IndexBuffer meshIndexBuffer,
            PrimitiveType primitiveType,
            Matrix[] instanceWorldTransforms,
            Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers);

        IRenderRequest CreateInstancedRequestFromState(
            IRenderContext renderContext,
            IEffect effect,
            IEffectParameterSet effectParameterSet,
            VertexBuffer meshVertexBuffer, 
            IndexBuffer meshIndexBuffer,
            PrimitiveType primitiveType, 
            Matrix[] instancedWorldTransforms,
            Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers);
    }
}
