using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultRenderBatcher : IRenderBatcher
    {
        private readonly IProfiler _profiler;

        public DefaultRenderBatcher(IProfiler profiler)
        {
            _profiler = profiler;
        }

        private Dictionary<int, IRenderRequest> _requestLookup = new Dictionary<int, IRenderRequest>();

        private Dictionary<int, List<Matrix>> _requestInstances = new Dictionary<int, List<Matrix>>();

        private VertexBuffer _vertexBuffer;

        private int _vertexBufferLastInstanceCount;

        private readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4));

        public void QueueRequest(IRenderContext renderContext, IRenderRequest request)
        {
            using (_profiler.Measure("render-queue"))
            {
                if (renderContext.IsCurrentRenderPass<I3DBatchedRenderPass>())
                {
                    if (!_requestLookup.ContainsKey(request.StateHash))
                    {
                        _requestLookup[request.StateHash] = request;
                    }

                    if (!_requestInstances.ContainsKey(request.StateHash))
                    {
                        _requestInstances[request.StateHash] = new List<Matrix>();
                    }

                    _requestInstances[request.StateHash].AddRange(request.Instances);
                }
                else
                {
                    RenderRequestImmediate(renderContext, request);
                }
            }
        }

        public void FlushRequests(IRenderContext renderContext)
        {
            if (renderContext.IsCurrentRenderPass<I3DBatchedRenderPass>())
            {
                using (_profiler.Measure("render-flush"))
                {
                    foreach (var kv in _requestLookup)
                    {
                        if (_requestInstances[kv.Key].Count == 0)
                        {
                            continue;
                        }

                        var request = kv.Value;

                        int pc;
                        SetupForRequest(renderContext, request, out pc, false);

#if PLATFORM_WINDOWS
                        // TODO: Not quite working yet...
                        var allowInstancedCalls = false;
#else
                        var allowInstancedCalls = false;
#endif

                        if (allowInstancedCalls &&
                            renderContext.Effect.Techniques[request.TechniqueName + "Batched"] != null)
                        {
#if PLATFORM_WINDOWS
                            if (_vertexBuffer == null ||
                                _requestInstances[kv.Key].Count > _vertexBufferLastInstanceCount)
                            {
                                _vertexBuffer?.Dispose();

                                _vertexBuffer = new VertexBuffer(
                                    renderContext.GraphicsDevice,
                                    _vertexDeclaration,
                                    _requestInstances[kv.Key].Count,
                                    BufferUsage.WriteOnly);
                            }

                            var matrices = _requestInstances[kv.Key];
                            _vertexBuffer.SetData(matrices.ToArray(), 0, matrices.Count);
                            renderContext.GraphicsDevice.SetVertexBuffers(
                                new VertexBufferBinding(request.MeshVertexBuffer),
                                new VertexBufferBinding(_vertexBuffer, 0, 1));

                            foreach (var pass in renderContext.Effect.Techniques[request.TechniqueName].Passes)
                            {
                                pass.Apply();

                                renderContext.GraphicsDevice.DrawInstancedPrimitives(
                                    request.PrimitiveType,
                                    0,
                                    0,
                                    request.MeshVertexBuffer.VertexCount,
                                    0,
                                    pc,
                                    matrices.Count);
                            }
#endif
                        }
                        else
                        {
                            var oldWorld = renderContext.World;

                            renderContext.GraphicsDevice.SetVertexBuffer(request.MeshVertexBuffer);

                            foreach (var instance in _requestInstances[kv.Key])
                            {
                                // TODO: In instanced mode, we want to pass the world transforms through
                                // as a seperate vertex buffer.
                                renderContext.World = instance;

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
                        }

                        renderContext.PopEffect();

                        _requestInstances[kv.Key].Clear();
                    }

                    _requestLookup.Clear();
                    _requestInstances.Clear();
                }
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
            var clonedParameters = new List<EffectParameter>();

            foreach (var param in effect.Parameters)
            {
                var newParam = new EffectParameter(param);

                if (newParam.ParameterType == EffectParameterType.Texture2D)
                {
                    newParam.SetValue(param.GetValueTexture2D());
                }

                clonedParameters.Add(newParam);
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

        private void SetupForRequest(IRenderContext renderContext, IRenderRequest request, out int pc, bool setVertexBuffers)
        {
            renderContext.PushEffect(request.Effect);
            renderContext.GraphicsDevice.RasterizerState = request.RasterizerState;
            renderContext.GraphicsDevice.BlendState = request.BlendState;
            renderContext.GraphicsDevice.DepthStencilState = request.DepthStencilState;

            if (setVertexBuffers)
            {
                renderContext.GraphicsDevice.SetVertexBuffer(request.MeshVertexBuffer);
            }
            renderContext.GraphicsDevice.Indices = request.MeshIndexBuffer;
            
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
        }

        public void RenderRequestImmediate(IRenderContext renderContext, IRenderRequest request)
        {
            int pc;
            SetupForRequest(renderContext, request, out pc, true);

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