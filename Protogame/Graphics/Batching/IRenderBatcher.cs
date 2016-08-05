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

        void FlushRequests(IRenderContext renderContext);

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
            Matrix world);

        IRenderRequest CreateSingleRequestFromState(
            IRenderContext renderContext,
            IEffect effect,
            IEffectParameterSet effectParameterSet,
            VertexBuffer meshVertexBuffer, 
            IndexBuffer meshIndexBuffer,
            PrimitiveType primitiveType,
            Matrix world);

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
            Matrix[] instanceWorldTransforms);

        IRenderRequest CreateInstancedRequestFromState(
            IRenderContext renderContext,
            IEffect effect,
            IEffectParameterSet effectParameterSet,
            VertexBuffer meshVertexBuffer, 
            IndexBuffer meshIndexBuffer,
            PrimitiveType primitiveType, 
            Matrix[] instancedWorldTransforms);
    }
}
