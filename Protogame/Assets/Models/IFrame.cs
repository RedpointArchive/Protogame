using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IFrame
    {
        VertexPositionNormalTexture[] Vertexes
        {
            get;
        }

        int[] Indices
        {
            get;
        }

        VertexBuffer VertexBuffer { get; }

        IndexBuffer IndexBuffer { get; }

        void LoadBuffers(GraphicsDevice graphicsDevice);
    }
}
