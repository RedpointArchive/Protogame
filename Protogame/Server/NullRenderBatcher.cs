using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class NullRenderBatcher : IRenderBatcher
    {
        public ulong LastBatchCount { get; }
        public ulong LastApplyCount { get; }
        public ulong LastBatchSaveCount { get; }
        public void QueueRequest(IRenderContext renderContext, IRenderRequest request)
        {
            throw new NotImplementedException();
        }

        public void FlushRequests(IGameContext gameContext, IRenderContext renderContext)
        {
            throw new NotImplementedException();
        }

        public void RenderRequestImmediate(IRenderContext renderContext, IRenderRequest request)
        {
            throw new NotImplementedException();
        }

        public IRenderRequest CreateSingleRequest(IRenderContext renderContext, RasterizerState rasterizerState, BlendState blendState,
            DepthStencilState depthStencilState, IEffect effect, IEffectParameterSet effectParameterSet,
            VertexBuffer meshVertexBuffer, IndexBuffer meshIndexBuffer, PrimitiveType primitiveType, Matrix world,
            Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers)
        {
            throw new NotImplementedException();
        }

        public IRenderRequest CreateSingleRequestFromState(IRenderContext renderContext, IEffect effect,
            IEffectParameterSet effectParameterSet, VertexBuffer meshVertexBuffer, IndexBuffer meshIndexBuffer,
            PrimitiveType primitiveType, Matrix world, Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers)
        {
            throw new NotImplementedException();
        }

        public IRenderRequest CreateInstancedRequest(IRenderContext renderContext, RasterizerState rasterizerState,
            BlendState blendState, DepthStencilState depthStencilState, IEffect effect, IEffectParameterSet effectParameterSet,
            VertexBuffer meshVertexBuffer, IndexBuffer meshIndexBuffer, PrimitiveType primitiveType,
            Matrix[] instanceWorldTransforms, Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers)
        {
            throw new NotImplementedException();
        }

        public IRenderRequest CreateInstancedRequestFromState(IRenderContext renderContext, IEffect effect,
            IEffectParameterSet effectParameterSet, VertexBuffer meshVertexBuffer, IndexBuffer meshIndexBuffer,
            PrimitiveType primitiveType, Matrix[] instancedWorldTransforms, Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers)
        {
            throw new NotImplementedException();
        }
    }
}