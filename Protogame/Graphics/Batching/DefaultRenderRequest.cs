using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultRenderRequest : IRenderRequest
    {
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
            Matrix[] instances)
        {
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

            if (EffectParameterSet.HasSemantic<IWorldViewProjectionEffectSemantic>())
            {
                var semantic = EffectParameterSet.GetSemantic<IWorldViewProjectionEffectSemantic>();
                semantic.View = renderContext.View;
                semantic.Projection = renderContext.Projection;
                semantic.World = Matrix.Identity; // This is handled by the batcher; set it to identity so we can batch requests properly.
            }

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
    }
}