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

        Effect Effect { get; }

        string TechniqueName { get; }

        EffectParameter[] EffectParameters { get; }

        VertexBuffer MeshVertexBuffer { get; }

        IndexBuffer MeshIndexBuffer { get; }

        PrimitiveType PrimitiveType { get; }

        Matrix[] Instances { get; }
    }
}