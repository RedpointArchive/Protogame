using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultRenderBatcher : IRenderBatcher
    {
        private Queue<IRenderRequest> _requestQueue = new Queue<IRenderRequest>();

        public void QueueRequest(IRenderContext renderContext, IRenderRequest request)
        {
            if (renderContext.IsCurrentRenderPass<I3DBatchedRenderPass>())
            {
                _requestQueue.Enqueue(request);
            }
            else
            {
                RenderRequestImmediate(renderContext, request);
            }
        }

        public void FlushRequests(IRenderContext renderContext)
        {
            if (renderContext.IsCurrentRenderPass<I3DBatchedRenderPass>())
            {
                // TODO
            }
        }

        public IRenderRequest CreateSingleRequest(
            IRenderContext renderContext,
            RasterizerState rasterizerState, 
            BlendState blendState,
            DepthStencilState depthStencilState,
            Effect effect,
            EffectParameter[] effectParameters,
            VertexBuffer meshVertexBuffer,
            IndexBuffer meshIndexBuffer, 
            PrimitiveType primitiveType,
            Matrix world)
        {
            return new DefaultRenderRequest(
                rasterizerState,
                blendState,
                depthStencilState,
                effect,
                renderContext.GetCurrentRenderPass<IRenderPass>().EffectTechniqueName,
                effectParameters,
                meshVertexBuffer,
                meshIndexBuffer,
                primitiveType,
                new [] { world });
        }

        public IRenderRequest CreateSingleRequestFromState(IRenderContext renderContext, VertexBuffer meshVertexBuffer,
            IndexBuffer meshIndexBuffer, PrimitiveType primitiveType, Matrix world)
        {
            return CreateSingleRequest(
                renderContext,
                renderContext.GraphicsDevice.RasterizerState,
                renderContext.GraphicsDevice.BlendState,
                renderContext.GraphicsDevice.DepthStencilState,
                renderContext.Effect,
                CloneParameters(renderContext.Effect),
                meshVertexBuffer,
                meshIndexBuffer,
                primitiveType,
                world);
        }

        private EffectParameter[] CloneParameters(Effect effect)
        {
            var clonedParameters = effect.Clone().Parameters;

            foreach (var param in clonedParameters)
            {
                if (param.ParameterType == EffectParameterType.Texture2D)
                {
                    param.SetValue(effect.Parameters[param.Name].GetValueTexture2D());
                }
            }

            return clonedParameters.ToArray();
        }

        public IRenderRequest CreateInstancedRequest(
            IRenderContext renderContext,
            RasterizerState rasterizerState,
            BlendState blendState, 
            DepthStencilState depthStencilState,
            Effect effect,
            EffectParameter[] effectParameters,
            VertexBuffer meshVertexBuffer, 
            IndexBuffer meshIndexBuffer, 
            PrimitiveType primitiveType,
            Matrix[] instanceWorldTransforms)
        {
            return new DefaultRenderRequest(
                rasterizerState,
                blendState,
                depthStencilState,
                effect,
                renderContext.GetCurrentRenderPass<IRenderPass>().EffectTechniqueName,
                effectParameters,
                meshVertexBuffer,
                meshIndexBuffer,
                primitiveType,
                instanceWorldTransforms);
        }

        public IRenderRequest CreateInstancedRequestFromState(IRenderContext renderContext, VertexBuffer meshVertexBuffer,
            IndexBuffer meshIndexBuffer, PrimitiveType primitiveType, Matrix[] instancedWorldTransforms)
        {
            return CreateInstancedRequest(
                renderContext,
                renderContext.GraphicsDevice.RasterizerState,
                renderContext.GraphicsDevice.BlendState,
                renderContext.GraphicsDevice.DepthStencilState,
                renderContext.Effect,
                CloneParameters(renderContext.Effect),
                meshVertexBuffer,
                meshIndexBuffer,
                primitiveType,
                instancedWorldTransforms);
        }

        public void RenderRequestImmediate(IRenderContext renderContext, IRenderRequest request)
        {
            renderContext.PushEffect(request.Effect);
            renderContext.GraphicsDevice.RasterizerState = request.RasterizerState;
            renderContext.GraphicsDevice.BlendState = request.BlendState;
            renderContext.GraphicsDevice.DepthStencilState = request.DepthStencilState;

            renderContext.GraphicsDevice.SetVertexBuffer(request.MeshVertexBuffer);
            renderContext.GraphicsDevice.Indices = request.MeshIndexBuffer;
            
            var pc = 0;
            switch (request.PrimitiveType)
            {
                case PrimitiveType.TriangleStrip:
                    pc = request.MeshVertexBuffer.VertexCount - 2;
                    break;
                case PrimitiveType.TriangleList:
                    pc = request.MeshVertexBuffer.VertexCount / 3;
                    break;
                case PrimitiveType.LineStrip:
                    pc = request.MeshVertexBuffer.VertexCount - 1;
                    break;
                case PrimitiveType.LineList:
                    pc = request.MeshVertexBuffer.VertexCount / 2;
                    break;
                default:
                    throw new InvalidOperationException("Unknown primitive type!");
            }

            for (var i = 0; i < request.EffectParameters.Length; i++)
            {
                var src = request.EffectParameters[i];
                var dest = renderContext.Effect.Parameters[src.Name];
                switch (dest.ParameterType)
                {
                    case EffectParameterType.Bool:
                        dest.SetValue(src.GetValueBoolean());
                        break;
                    case EffectParameterType.Int32:
                        dest.SetValue(src.GetValueInt32());
                        break;
                    case EffectParameterType.Single:
                        if (dest.ParameterClass == EffectParameterClass.Scalar)
                        {
                            dest.SetValue(src.GetValueSingle());
                        }
                        else if (dest.ParameterClass == EffectParameterClass.Matrix)
                        {
                            if (dest.Elements.Count > 0)
                            {
                                dest.SetValue(src.GetValueMatrixArray(dest.Elements.Count));
                            }
                            else
                            {
                                dest.SetValue(src.GetValueMatrix());
                            }
                        }
                        else if (dest.ParameterClass == EffectParameterClass.Vector)
                        {
                            if (dest.ColumnCount == 4)
                            {
                                if (dest.Elements.Count > 0)
                                {
                                    dest.SetValue(src.GetValueVector4Array());
                                }
                                else
                                {
                                    dest.SetValue(src.GetValueVector4());
                                }
                            }
                            else if (dest.ColumnCount == 3)
                            {
                                if (dest.Elements.Count > 0)
                                {
                                    dest.SetValue(src.GetValueVector3Array());
                                }
                                else
                                {
                                    dest.SetValue(src.GetValueVector3());
                                }
                            }
                            else if (dest.ColumnCount == 2)
                            {
                                if (dest.Elements.Count > 0)
                                {
                                    dest.SetValue(src.GetValueVector2Array());
                                }
                                else
                                {
                                    dest.SetValue(src.GetValueVector2());
                                }
                            }
                        }
                        else
                        {
                            dest.SetValue(src.GetValueSingleArray());
                        }
                        break;
                    case EffectParameterType.Texture2D:
                        dest.SetValue(src.GetValueTexture2D());
                        break;
                    case EffectParameterType.Texture3D:
                        dest.SetValue(src.GetValueTexture3D());
                        break;
                    case EffectParameterType.Void:
                        break;
                    default:
                        throw new InvalidOperationException("Can't copy value to target effect (dest expected type " + dest.ParameterType + ").");
                }
            }

            var oldWorld = renderContext.World;

            for (var i = 0; i < request.Instances.Length; i++)
            {
                // TODO: In instanced mode, we want to pass the world transforms through
                // as a seperate vertex buffer.
                renderContext.World = request.Instances[i];

                foreach (var pass in renderContext.Effect.Techniques[request.TechniqueName].Passes)
                {
                    pass.Apply();
                    
                    renderContext.GraphicsDevice.DrawIndexedPrimitives(
                        request.PrimitiveType,
                        0,
                        0,
                        pc);
                }
            }

            renderContext.World = oldWorld;

            renderContext.PopEffect();
        }
    }
}