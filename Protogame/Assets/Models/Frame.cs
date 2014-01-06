using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    class Frame : IFrame
    {
        private VertexBuffer m_VertexBuffer;

        private IndexBuffer m_IndexBuffer;

        public Frame(VertexPositionNormalTexture[] vertexes, int[] indices)
        {
            this.Vertexes = vertexes;
            this.Indices = indices;
        }

        public VertexPositionNormalTexture[] Vertexes { get; private set; }

        public int[] Indices { get; private set; }

        public VertexBuffer VertexBuffer
        {
            get
            {
                if (this.m_VertexBuffer == null)
                {
                    throw new InvalidOperationException("Call LoadBuffers before accessing the vertex buffer");
                }

                return this.m_VertexBuffer;
            }
            private set
            {
                this.m_VertexBuffer = value;
            }
        }

        public IndexBuffer IndexBuffer
        {
            get
            {
                if (this.m_IndexBuffer == null)
                {
                    throw new InvalidOperationException("Call LoadBuffers before accessing the index buffer");
                }

                return this.m_IndexBuffer;
            }
            private set
            {
                this.m_IndexBuffer = value;
            }
        }

        public void LoadBuffers(GraphicsDevice graphicsDevice)
        {
            if (this.m_VertexBuffer == null)
            {
                this.m_VertexBuffer = new VertexBuffer(
                    graphicsDevice,
                    VertexPositionNormalTexture.VertexDeclaration,
                    this.Vertexes.Length,
                    BufferUsage.WriteOnly);
                this.m_VertexBuffer.SetData(this.Vertexes);
            }

            if (this.m_IndexBuffer == null)
            {
                this.m_IndexBuffer = new IndexBuffer(
                    graphicsDevice,
                    IndexElementSize.ThirtyTwoBits,
                    this.Indices.Length,
                    BufferUsage.WriteOnly);
                this.m_IndexBuffer.SetData(this.Indices);
            }
        }
    }
}
