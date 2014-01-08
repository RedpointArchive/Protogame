namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An implementation of <see cref="IFrame"/>.
    /// </summary>
    internal class Frame : IFrame
    {
        /// <summary>
        /// The index buffer.
        /// </summary>
        private IndexBuffer m_IndexBuffer;

        /// <summary>
        /// The vertex buffer.
        /// </summary>
        private VertexBuffer m_VertexBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
        /// <param name="vertexes">
        /// The vertexes.
        /// </param>
        /// <param name="indices">
        /// The indices.
        /// </param>
        public Frame(VertexPositionNormalTexture[] vertexes, int[] indices)
        {
            this.Vertexes = vertexes;
            this.Indices = indices;
        }

        /// <summary>
        /// Gets the index buffer.
        /// </summary>
        /// <value>
        /// The index buffer.
        /// </value>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the vertex or index buffers have not been loaded with <see cref="LoadBuffers"/>.
        /// </exception>
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

        /// <summary>
        /// Gets the indices.
        /// </summary>
        /// <value>
        /// The indices.
        /// </value>
        public int[] Indices { get; private set; }

        /// <summary>
        /// Gets the vertex buffer.
        /// </summary>
        /// <value>
        /// The vertex buffer.
        /// </value>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the vertex or index buffers have not been loaded with <see cref="LoadBuffers"/>.
        /// </exception>
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

        /// <summary>
        /// Gets the vertexes.
        /// </summary>
        /// <value>
        /// The vertexes.
        /// </value>
        public VertexPositionNormalTexture[] Vertexes { get; private set; }

        /// <summary>
        /// The load buffers.
        /// </summary>
        /// <param name="graphicsDevice">
        /// The graphics device.
        /// </param>
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