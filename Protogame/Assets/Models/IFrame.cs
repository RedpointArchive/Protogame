namespace Protogame
{
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An interface representing a frame in an animation.
    /// </summary>
    public interface IFrame
    {
        /// <summary>
        /// Gets the index buffer.
        /// </summary>
        /// <value>
        /// The index buffer.
        /// </value>
        IndexBuffer IndexBuffer { get; }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        /// <value>
        /// The indices.
        /// </value>
        int[] Indices { get; }

        /// <summary>
        /// Gets the vertex buffer.
        /// </summary>
        /// <value>
        /// The vertex buffer.
        /// </value>
        VertexBuffer VertexBuffer { get; }

        /// <summary>
        /// Gets the vertexes.
        /// </summary>
        /// <value>
        /// The vertexes.
        /// </value>
        VertexPositionNormalTexture[] Vertexes { get; }

        /// <summary>
        /// The load buffers.
        /// </summary>
        /// <param name="graphicsDevice">
        /// The graphics device.
        /// </param>
        void LoadBuffers(GraphicsDevice graphicsDevice);
    }
}