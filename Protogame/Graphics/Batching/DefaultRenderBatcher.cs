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
        private readonly IRenderAutoCache _renderAutoCache;

        public DefaultRenderBatcher(IProfiler profiler, IRenderAutoCache renderAutoCache)
        {
            _profiler = profiler;
            _renderAutoCache = renderAutoCache;
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
        
        public ulong LastBatchCount { get; private set; }

        public ulong LastApplyCount { get; private set; }

        public ulong LastBatchSaveCount { get; private set; }

        public void QueueRequest(IRenderContext renderContext, IRenderRequest request)
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

                if (request.Instances.Length > 1)
                {
                    _requestInstances[request.StateHash].AddRange(request.Instances);
                }
                else
                {
                    _requestInstances[request.StateHash].Add(request.Instances[0]);
                }
            }
            else
            {
                RenderRequestImmediate(renderContext, request);
            }
        }

        public void FlushRequests(IGameContext gameContext, IRenderContext renderContext)
        {
            LastBatchCount = 0;
            LastApplyCount = 0;
            LastBatchSaveCount = 0;

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

                        LastBatchCount++;
                        LastBatchSaveCount += (ulong)(_requestInstances[kv.Key].Count - 1);

                        var request = kv.Value;

                        int pc;
                        SetupForRequest(renderContext, request, out pc, false);
                        request.Effect.NativeEffect.Parameters["View"]?.SetValue(renderContext.View);
                        request.Effect.NativeEffect.Parameters["Projection"]?.SetValue(renderContext.Projection);

#if PLATFORM_WINDOWS
                        var allowInstancedCalls = true;
#else
                        var allowInstancedCalls = false;
#endif
                        
                        if (allowInstancedCalls &&
                            request.Effect.NativeEffect.Techniques[request.TechniqueName + "Batched"] != null)
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
                                _vertexBufferLastInstanceCount = _requestInstances[kv.Key].Count;
                            }

                            var matrices = _requestInstances[kv.Key];
                            _vertexBuffer.SetData(matrices.ToArray(), 0, matrices.Count);
                            renderContext.GraphicsDevice.SetVertexBuffers(
                                new VertexBufferBinding(request.MeshVertexBuffer),
                                new VertexBufferBinding(_vertexBuffer, 0, 1));

                            foreach (var pass in request.Effect.NativeEffect.Techniques[request.TechniqueName + "Batched"].Passes)
                            {
                                pass.Apply();

                                renderContext.GraphicsDevice.DrawInstancedPrimitives(
                                    request.PrimitiveType,
                                    0,
                                    0,
                                    pc,
                                    matrices.Count);
                            }
#endif
                        }
                        else
                        {
                            // If there's less than 5 instances, just push the draw calls to the GPU.
                            if (_requestInstances[kv.Key].Count <= 5 || !request.SupportsComputingInstancesToCustomBuffers)
                            {
                                renderContext.GraphicsDevice.SetVertexBuffer(request.MeshVertexBuffer);

                                foreach (var instance in _requestInstances[kv.Key])
                                {
                                    request.Effect.NativeEffect.Parameters["World"]?.SetValue(instance);

                                    foreach (var pass in request.Effect.NativeEffect.Techniques[request.TechniqueName].Passes)
                                    {
                                        pass.Apply();

                                        LastApplyCount++;

                                        renderContext.GraphicsDevice.DrawIndexedPrimitives(
                                            request.PrimitiveType,
                                            0,
                                            0,
                                            pc);
                                    }
                                }
                            }
                            else
                            {
                                var buffersNeedComputing = false;
                                var vertexBuffer = _renderAutoCache.AutoCache("renderbatcher-" + kv.Key, new object[]
                                {
                                    _requestInstances[kv.Key].Count,
                                    request.MeshVertexBuffer.VertexCount,
                                    request.MeshVertexBuffer.VertexDeclaration
                                }, gameContext, () =>
                                {
                                    buffersNeedComputing = true;
                                    return new VertexBuffer(
                                        renderContext.GraphicsDevice,
                                        request.MeshVertexBuffer.VertexDeclaration,
                                        _requestInstances[kv.Key].Count*request.MeshVertexBuffer.VertexCount,
                                        BufferUsage.WriteOnly);
                                });
                                var indexBuffer = _renderAutoCache.AutoCache("renderbatcher-" + kv.Key, new object[]
                                {
                                    _requestInstances[kv.Key].Count,
                                    request.MeshVertexBuffer.VertexCount,
                                }, gameContext, () =>
                                {
                                    buffersNeedComputing = true;
                                    return new IndexBuffer(
                                        renderContext.GraphicsDevice,
                                        IndexElementSize.ThirtyTwoBits,
                                        _requestInstances[kv.Key].Count*request.MeshIndexBuffer.IndexCount,
                                        BufferUsage.WriteOnly);
                                });

                                if (buffersNeedComputing)
                                {
                                    // Compute a pre-transformed vertex and index buffer for rendering.
                                    request.ComputeInstancesToCustomBuffers(
                                        _requestInstances[kv.Key],
                                        vertexBuffer,
                                        indexBuffer);
                                }

                                renderContext.GraphicsDevice.SetVertexBuffer(vertexBuffer);
                                renderContext.GraphicsDevice.Indices = indexBuffer;
                                request.Effect.NativeEffect.Parameters["World"]?.SetValue(Matrix.Identity);

                                foreach (
                                    var pass in
                                        request.Effect.NativeEffect.Techniques[request.TechniqueName].Passes
                                    )
                                {
                                    pass.Apply();

                                    renderContext.GraphicsDevice.DrawIndexedPrimitives(
                                        request.PrimitiveType,
                                        0,
                                        0,
                                        pc * _requestInstances[kv.Key].Count);
                                }
                            }
                        }

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
            IEffect effect,
            IEffectParameterSet effectParameterSet,
            VertexBuffer meshVertexBuffer, 
            IndexBuffer meshIndexBuffer,
            PrimitiveType primitiveType,
            Matrix world, 
            Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers)
        {
            return new DefaultRenderRequest(
                renderContext,
                rasterizerState,
                blendState,
                depthStencilState,
                effect,
                renderContext.GetCurrentRenderPass<IRenderPass>().EffectTechniqueName,
                effectParameterSet,
                meshVertexBuffer,
                meshIndexBuffer,
                primitiveType,
                new [] { world },
                computeCombinedBuffers);
        }

        public IRenderRequest CreateSingleRequestFromState(
            IRenderContext renderContext,
            IEffect effect, 
            IEffectParameterSet effectParameterSet, 
            VertexBuffer meshVertexBuffer, 
            IndexBuffer meshIndexBuffer, 
            PrimitiveType primitiveType, 
            Matrix world, 
            Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers)
        {
            return CreateSingleRequest(
                renderContext,
                renderContext.GraphicsDevice.RasterizerState,
                renderContext.GraphicsDevice.BlendState,
                renderContext.GraphicsDevice.DepthStencilState,
                effect,
                effectParameterSet,
                meshVertexBuffer,
                meshIndexBuffer,
                primitiveType,
                world, 
                computeCombinedBuffers);
        }
        
        public IRenderRequest CreateInstancedRequest(
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
            Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers)
        {
            return new DefaultRenderRequest(
                renderContext,
                rasterizerState,
                blendState,
                depthStencilState,
                effect,
                renderContext.GetCurrentRenderPass<IRenderPass>().EffectTechniqueName,
                effectParameterSet,
                meshVertexBuffer,
                meshIndexBuffer,
                primitiveType,
                instanceWorldTransforms,
                computeCombinedBuffers);
        }

        public IRenderRequest CreateInstancedRequestFromState(
            IRenderContext renderContext, 
            IEffect effect,
            IEffectParameterSet effectParameterSet,
            VertexBuffer meshVertexBuffer,
            IndexBuffer meshIndexBuffer,
            PrimitiveType primitiveType,
            Matrix[] instancedWorldTransforms,
            Action<List<Matrix>, VertexBuffer, IndexBuffer> computeCombinedBuffers)
        {
            return CreateInstancedRequest(
                renderContext,
                renderContext.GraphicsDevice.RasterizerState,
                renderContext.GraphicsDevice.BlendState,
                renderContext.GraphicsDevice.DepthStencilState,
                effect,
                effectParameterSet,
                meshVertexBuffer,
                meshIndexBuffer,
                primitiveType,
                instancedWorldTransforms,
                computeCombinedBuffers);
        }

        private void SetupForRequest(IRenderContext renderContext, IRenderRequest request, out int pc, bool setVertexBuffers)
        {
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
                    pc = request.MeshIndexBuffer.IndexCount - 2;
                    break;
                case PrimitiveType.TriangleList:
                    pc = request.MeshIndexBuffer.IndexCount / 3;
                    break;
                case PrimitiveType.LineStrip:
                    pc = request.MeshIndexBuffer.IndexCount - 1;
                    break;
                case PrimitiveType.LineList:
                    pc = request.MeshIndexBuffer.IndexCount / 2;
                    break;
                default:
                    throw new InvalidOperationException("Unknown primitive type!");
            }

            request.Effect.LoadParameterSet(renderContext, request.EffectParameterSet, true);
            request.Effect.NativeEffect.Parameters["View"]?.SetValue(renderContext.View);
            request.Effect.NativeEffect.Parameters["Projection"]?.SetValue(renderContext.Projection);
        }

        public void RenderRequestImmediate(IRenderContext renderContext, IRenderRequest request)
        {
            int pc;
            SetupForRequest(renderContext, request, out pc, true);
            
            for (var i = 0; i < request.Instances.Length; i++)
            {
                request.Effect.NativeEffect.Parameters["World"]?.SetValue(request.Instances[i]);

                foreach (var pass in request.Effect.NativeEffect.Techniques[request.TechniqueName].Passes)
                {
                    pass.Apply();
                    
                    renderContext.GraphicsDevice.DrawIndexedPrimitives(
                        request.PrimitiveType,
                        0,
                        0,
                        pc);
                }
            }
        }
    }
}