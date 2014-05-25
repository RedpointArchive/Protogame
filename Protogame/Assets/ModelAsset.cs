namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Represents a model asset.
    /// </summary>
    public class ModelAsset : MarshalByRefObject, IAsset, IModel
    {
        /// <summary>
        /// The runtime model associated with this asset.
        /// </summary>
        private Model m_Model;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAsset"/> class.
        /// <para>
        /// This constructor is called by <see cref="ModelAssetLoader"/> when loading a model asset.
        /// </para>
        /// </summary>
        /// <param name="name">
        /// The name of the model asset.
        /// </param>
        /// <param name="rawData">
        /// The raw, serialized model data.
        /// </param>
        /// <param name="rawAdditionalAnimations">
        /// The source raw data that contains additional animations, mapping byte arrays to animation names.
        /// </param>
        /// <param name="data">
        /// The platform specific data.
        /// </param>
        /// <param name="sourcedFromRaw">
        /// Whether or not this asset was sourced from a purely raw asset file (such as a PNG).
        /// </param>
        /// <param name="extension">
        /// The appropriate file extension for this model.
        /// </param>
        public ModelAsset(
            string name, 
            byte[] rawData, 
            Dictionary<string, byte[]> rawAdditionalAnimations, 
            PlatformData data, 
            bool sourcedFromRaw,
            string extension)
        {
            this.Name = name;
            this.RawData = rawData;
            this.RawAdditionalAnimations = rawAdditionalAnimations;
            this.PlatformData = data;
            this.SourcedFromRaw = sourcedFromRaw;
            this.Extension = extension;

            if (this.PlatformData != null)
            {
                try
                {
                    this.ReloadModel();
                }
                catch (NoAssetContentManagerException)
                {
                }
            }
        }

        /// <summary>
        /// Gets the available animations.
        /// </summary>
        /// <value>
        /// The available animations.
        /// </value>
        public IAnimationCollection AvailableAnimations
        {
            get
            {
                return this.m_Model.AvailableAnimations;
            }
        }

        /// <summary>
        /// Gets the root bone of the model's skeleton.
        /// </summary>
        /// <remarks>
        /// This value is null if there is no skeleton attached to the model.
        /// </remarks>
        /// <value>
        /// The root bone of the model's skeleton.
        /// </value>
        public IModelBone Root
        {
            get
            {
                return this.m_Model.Root;
            }
        }

        /// <summary>
        /// Gets the model's bones by their names.
        /// </summary>
        /// <remarks>
        /// This value is null if there is no skeleton attached to the model.
        /// </remarks>
        /// <value>
        /// The model bones addressed by their names.
        /// </value>
        public IDictionary<string, IModelBone> Bones
        {
            get
            {
                return this.m_Model.Bones;
            }
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
                return this.m_Model.IndexBuffer;
            }
        }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        /// <value>
        /// The indices.
        /// </value>
        public int[] Indices
        {
            get
            {
                return this.m_Model.Indices;
            }
        }

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
                return this.m_Model.VertexBuffer;
            }
        }

        /// <summary>
        /// Gets the vertexes.
        /// </summary>
        /// <value>
        /// The vertexes.
        /// </value>
        public VertexPositionNormalTextureBlendable[] Vertexes
        {
            get
            {
                return this.m_Model.Vertexes;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the asset only contains compiled information.
        /// </summary>
        /// <value>
        /// Whether the asset only contains compiled information.
        /// </value>
        public bool CompiledOnly
        {
            get
            {
                return this.RawData == null;
            }
        }

        /// <summary>
        /// Gets the name of the asset.
        /// </summary>
        /// <value>
        /// The name of the asset.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the platform-specific data associated with this asset.
        /// </summary>
        /// <seealso cref="PlatformData"/>
        /// <value>
        /// The platform-specific data for this asset.
        /// </value>
        public PlatformData PlatformData { get; set; }

        /// <summary>
        /// Gets or sets the raw additional animation data.  This is populated when loading models from
        /// raw FBX files, and there are multiple FBX files for the one model.
        /// </summary>
        /// <value>
        /// The raw additional animation data.
        /// </value>
        public Dictionary<string, byte[]> RawAdditionalAnimations { get; set; }

        /// <summary>
        /// Gets or sets the raw model data.  This is the source information used to compile the asset.
        /// </summary>
        /// <value>
        /// The raw texture data.
        /// </value>
        public byte[] RawData { get; set; }

        /// <summary>
        /// Gets or sets the appropriate file extension for the raw model data.  This is only used during model
        /// compilation.
        /// </summary>
        /// <value>
        /// The file extension for the raw model data.
        /// </value>
        public string Extension { get; set; }

        /// <summary>
        /// Gets a value indicating whether the asset only contains source information.
        /// </summary>
        /// <value>
        /// Whether the asset only contains source information.
        /// </value>
        public bool SourceOnly
        {
            get
            {
                return this.PlatformData == null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this asset was sourced from a raw file (such as a FBX file).
        /// </summary>
        /// <value>
        /// The sourced from raw.
        /// </value>
        public bool SourcedFromRaw { get; private set; }

        /// <summary>
        /// Modifies the specified model to align to this animation at the specified frame and then renders it.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="transform">The world transformation to apply.</param>
        /// <param name="animationName">The animation to play.</param>
        /// <param name="secondFraction">The time elapsed.</param>
        /// <param name="multiply">The multiplication factor to apply to the animation speed.</param>
        public void Render(IRenderContext renderContext, Matrix transform, string animationName, TimeSpan secondFraction, float multiply = 1)
        {
            this.m_Model.LoadBuffers(renderContext.GraphicsDevice);

            if (animationName != null)
            {
                this.m_Model.AvailableAnimations[animationName].Render(
                    renderContext,
                    transform,
                    this.m_Model,
                    secondFraction,
                    multiply);
            }
            else
            {
                this.m_Model.Render(renderContext, transform);
            }
        }

        /// <summary>
        /// Modifies the specified model to align to this animation at the specified frame and then renders it.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="transform">The world transformation to apply.</param>
        /// <param name="animationName">The animation to play.</param>
        /// <param name="frame">The frame to draw at.</param>
        public void Render(IRenderContext renderContext, Matrix transform, string animationName, double frame)
        {
            this.m_Model.LoadBuffers(renderContext.GraphicsDevice);

            if (animationName != null)
            {
                this.m_Model.AvailableAnimations[animationName].Render(
                    renderContext,
                    transform,
                    this.m_Model,
                    frame);
            }
            else
            {
                this.m_Model.Render(renderContext, transform);
            }
        }

        /// <summary>
        /// Renders the specified model in it's current state.
        /// </summary>
        /// <param name="renderContext">The current render context.</param>
        /// <param name="transform">The world transformation to apply.</param>
        public void Render(IRenderContext renderContext, Matrix transform)
        {
            this.m_Model.LoadBuffers(renderContext.GraphicsDevice);

            this.m_Model.Render(renderContext, transform);
        }

        /// <summary>
        /// Loads vertex and index buffers for all of animations in this model.
        /// </summary>
        /// <param name="graphicsDevice">
        /// The graphics device.
        /// </param>
        public void LoadBuffers(GraphicsDevice graphicsDevice)
        {
            this.m_Model.LoadBuffers(graphicsDevice);
        }

        /// <summary>
        /// Reloads the model from the associated compiled data.
        /// </summary>
        public void ReloadModel()
        {
            if (this.PlatformData != null)
            {
                var serializer = new ModelSerializer();
                this.m_Model = serializer.Deserialize(this.PlatformData.Data);
            }
        }

        /// <summary>
        /// Attempt to resolve this asset to the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The target type of the asset.
        /// </typeparam>
        /// <returns>
        /// The current asset as a <see cref="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the current asset can not be casted to the designated type.
        /// </exception>
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(ModelAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to ModelAsset.");
        }
    }
}