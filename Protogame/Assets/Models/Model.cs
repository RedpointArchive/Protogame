namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using PrimitiveType = Microsoft.Xna.Framework.Graphics.PrimitiveType;

    /// <summary>
    /// This represents a runtime model, with full support for animation and bone manipulation.
    /// </summary>
    public class Model : IModel
    {
        /// <summary>
        /// The flattened version of the bone structures.
        /// </summary>
        private readonly IModelBone[] m_FlattenedBones;

        /// <summary>
        /// The index buffer.
        /// </summary>
        private IndexBuffer m_IndexBuffer;

        /// <summary>
        /// The vertex buffer.
        /// </summary>
        private VertexBuffer m_VertexBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="availableAnimations">
        /// The available animations.
        /// </param>
        /// <param name="rootBone">
        /// The root bone, or null if there's no skeletal information.
        /// </param>
        /// <param name="vertexes">
        /// The vertexes associated with this model.
        /// </param>
        /// <param name="indices">
        /// The indices associated with the model.
        /// </param>
        public Model(
            IAnimationCollection availableAnimations,
            IModelBone rootBone,
            VertexPositionNormalTextureBlendable[] vertexes,
            int[] indices)
        {
            this.AvailableAnimations = availableAnimations;
            this.Root = rootBone;
            this.Vertexes = vertexes;
            this.Indices = indices;

            if (this.Root != null)
            {
                this.m_FlattenedBones = this.Root.Flatten();
                this.Bones = this.m_FlattenedBones.ToDictionary(k => k.Name, v => v);
            }
        }

        /// <summary>
        /// Gets the available animations.
        /// </summary>
        /// <value>
        /// The available animations.
        /// </value>
        public IAnimationCollection AvailableAnimations { get; private set; }

        /// <summary>
        /// Gets the root bone of the model's skeleton.
        /// </summary>
        /// <remarks>
        /// This value is null if there is no skeleton attached to the model.
        /// </remarks>
        /// <value>
        /// The root bone of the model's skeleton.
        /// </value>
        public IModelBone Root { get; private set; }

        /// <summary>
        /// Gets the model's bones by their names.
        /// </summary>
        /// <remarks>
        /// This value is null if there is no skeleton attached to the model.
        /// </remarks>
        /// <value>
        /// The model bones addressed by their names.
        /// </value>
        public IDictionary<string, IModelBone> Bones { get; private set; }

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
        }

        /// <summary>
        /// Gets the indices of the model.
        /// </summary>
        /// <value>
        /// The indices of the model.
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
        }

        /// <summary>
        /// Gets the vertexes of the model.
        /// </summary>
        /// <value>
        /// The vertexes of the model.
        /// </value>
        public VertexPositionNormalTextureBlendable[] Vertexes { get; private set; }

        /// <summary>
        /// Draws the model using the specified animation, calculating the appropriate frame to play
        /// based on how much time has elapsed.
        /// </summary>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        public void Render(IRenderContext renderContext, Matrix transform)
        {
            if (this.Vertexes.Length == 0 && this.Indices.Length == 0)
            {
                throw new InvalidOperationException(
                    "This model does not have any vertexes or indices.  It's most " +
                    "likely been imported from an FBX file that only contains hierarchy, " +
                    "in which case there isn't anything to render.");
            }

            this.LoadBuffers(renderContext.GraphicsDevice);

            var effectBones = renderContext.Effect as IEffectBones;

            if (effectBones == null)
            {
                throw new InvalidOperationException(
                    "The current effect on the render context does " +
                    "not implement IEffectBones.  You can use " +
                    "'effect.Skinned' for a basic model rendering effect.");
            }

            foreach (var bone in this.m_FlattenedBones)
            {
                if (bone.ID == -1)
                {
                    continue;
                }

                effectBones.Bones[bone.ID] = bone.GetFinalMatrix();
            }

            // Keep a copy of the current world transformation and then apply the
            // transformation that was passed in.
            var oldWorld = renderContext.World;
            renderContext.World *= transform;

            // Render the vertex and index buffer.
            renderContext.GraphicsDevice.Indices = this.IndexBuffer;
            renderContext.GraphicsDevice.SetVertexBuffer(this.VertexBuffer);
            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                renderContext.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    this.VertexBuffer.VertexCount,
                    0,
                    this.IndexBuffer.IndexCount / 3);
            }

            // Restore the world matrix.
            renderContext.World = oldWorld;
        }
        
        /// <summary>
        /// Load the vertex and index buffer for this model.
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
                    VertexPositionNormalTextureBlendable.VertexDeclaration, 
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