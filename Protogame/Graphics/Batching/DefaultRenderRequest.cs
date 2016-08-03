using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultRenderRequest : IRenderRequest
    {
        public DefaultRenderRequest(
            RasterizerState rasterizerState,
            BlendState blendState,
            DepthStencilState depthStencilState,
            Effect effect,
            string techniqueName,
            EffectParameter[] effectParameters,
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
            EffectParameters = effectParameters;
            MeshVertexBuffer = meshVertexBuffer;
            MeshIndexBuffer = meshIndexBuffer;
            PrimitiveType = primitiveType;
            Instances = instances;

            StateHash =
                RasterizerState.GetHashCode() ^ 397 +
                BlendState.GetHashCode() ^ 397 +
                DepthStencilState.GetHashCode() ^ 397 +
                Effect.GetHashCode() ^ 397 +
                TechniqueName.GetHashCode() ^ 397 +
                EffectParameters.GetHashCode() ^ 397 +
                MeshVertexBuffer.GetHashCode() ^ 397 +
                MeshIndexBuffer.GetHashCode() ^ 397 +
                PrimitiveType.GetHashCode() ^ 397;
        }

        public int StateHash { get; }

        public RasterizerState RasterizerState { get; }

        public BlendState BlendState { get; }

        public DepthStencilState DepthStencilState { get; }

        public Effect Effect { get; }

        public string TechniqueName { get; }

        public EffectParameter[] EffectParameters { get; }

        public VertexBuffer MeshVertexBuffer { get; set; }

        public IndexBuffer MeshIndexBuffer { get; set; }

        public PrimitiveType PrimitiveType { get; set; }

        public Matrix[] Instances { get; }
    }
}