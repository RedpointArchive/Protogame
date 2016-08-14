using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IRenderRequest
    {
        int StateHash { get; }

        RasterizerState RasterizerState { get; }

        BlendState BlendState { get; }

        DepthStencilState DepthStencilState { get; }

        IEffect Effect { get; }

        string TechniqueName { get; }

        IEffectParameterSet EffectParameterSet { get; }

        VertexBuffer MeshVertexBuffer { get; }

        IndexBuffer MeshIndexBuffer { get; }

        PrimitiveType PrimitiveType { get; }

        Matrix[] Instances { get; }

        void ComputeInstancesToCustomBuffers(List<Matrix> matrices, VertexBuffer vertexBuffer, IndexBuffer indexBuffer);

        bool SupportsComputingInstancesToCustomBuffers { get; }
    }
}