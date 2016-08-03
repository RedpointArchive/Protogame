using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace ProtogameAssetTool
{
    public class NullRenderBatcher :IRenderBatcher
    {
        public void QueueRequest(IRenderContext renderContext, IRenderRequest request)
        {
            throw new NotImplementedException();
        }

        public void FlushRequests(IRenderContext renderContext)
        {
            throw new NotImplementedException();
        }

        public void RenderRequestImmediate(IRenderContext renderContext, IRenderRequest request)
        {
            throw new NotImplementedException();
        }

        public IRenderRequest CreateSingleRequest(IRenderContext renderContext, RasterizerState rasterizerState, BlendState blendState,
            DepthStencilState depthStencilState, Effect effect, EffectParameter[] effectParameters,
            VertexBuffer meshVertexBuffer, IndexBuffer meshIndexBuffer, PrimitiveType primitiveType, Matrix world)
        {
            throw new NotImplementedException();
        }

        public IRenderRequest CreateSingleRequestFromState(IRenderContext renderContext, VertexBuffer meshVertexBuffer,
            IndexBuffer meshIndexBuffer, PrimitiveType primitiveType, Matrix world)
        {
            throw new NotImplementedException();
        }

        public IRenderRequest CreateInstancedRequest(IRenderContext renderContext, RasterizerState rasterizerState,
            BlendState blendState, DepthStencilState depthStencilState, Effect effect, EffectParameter[] effectParameters,
            VertexBuffer meshVertexBuffer, IndexBuffer meshIndexBuffer, PrimitiveType primitiveType,
            Matrix[] instanceWorldTransforms)
        {
            throw new NotImplementedException();
        }

        public IRenderRequest CreateInstancedRequestFromState(IRenderContext renderContext, VertexBuffer meshVertexBuffer,
            IndexBuffer meshIndexBuffer, PrimitiveType primitiveType, Matrix[] instancedWorldTransforms)
        {
            throw new NotImplementedException();
        }
    }
}
