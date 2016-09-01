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
        private readonly IModelBone[] _flattenedBones;

        /// <summary>
        /// The model render configurations, which inform the model how it's vertices
        /// should be mapped to effects.
        /// </summary>
        private readonly IModelRenderConfiguration[] _modelRenderConfigurations;

        /// <summary>
        /// The render batcher, which is used to create render requests.
        /// </summary>
        private readonly IRenderBatcher _renderBatcher;

        /// <summary>
        /// The index buffer.
        /// </summary>
        private IndexBuffer _indexBuffer;

        /// <summary>
        /// The cached vertex buffers.
        /// </summary>
        private Dictionary<object, VertexBuffer> _cachedVertexBuffers;

        /// <summary>
        /// The cached model vertex mapping.
        /// </summary>
        private ModelVertexMapping _cachedModelVertexMapping;

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
            IModelRenderConfiguration[] modelRenderConfigurations,
            IRenderBatcher renderBatcher,
            string name,
            IAnimationCollection availableAnimations,
            IMaterial material,
            IModelBone rootBone,
            ModelVertex[] vertexes,
            int[] indices)
        {
            Name = name;
            AvailableAnimations = availableAnimations;
            Root = rootBone;
            Vertexes = vertexes;
            Indices = indices;
            Material = material;

            _cachedVertexBuffers = new Dictionary<object, VertexBuffer>();
            _modelRenderConfigurations = modelRenderConfigurations;
            _renderBatcher = renderBatcher;

            if (Root != null)
            {
                _flattenedBones = Root.Flatten();
                Bones = _flattenedBones.ToDictionary(k => k.Name, v => v);
            }
        }

        /// <summary>
        /// The name of the model, which usually aligns to the model asset that
        /// the model was loaded from.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the available animations.
        /// </summary>
        /// <value>
        /// The available animations.
        /// </value>
        public IAnimationCollection AvailableAnimations { get; private set; }

        /// <summary>
        /// Gets the material information associated with this model, if
        /// one exists.
        /// </summary>
        /// <remarks>
        /// This value is null if there is no material attached to this model.
        /// </remarks>
        /// <value>
        /// The material associated with this model.
        /// </value>
        public IMaterial Material { get; private set; }

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
                if (_indexBuffer == null)
                {
                    throw new InvalidOperationException("Call LoadBuffers before accessing the index buffer");
                }

                return _indexBuffer;
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
        /// Frees any vertex buffers that are cached inside this model.
        /// </summary>
        public void FreeCachedVertexBuffers()
        {
            foreach (var buffer in _cachedVertexBuffers)
            {
                buffer.Value.Dispose();
            }

            _cachedVertexBuffers.Clear();
        }

        /// <summary>
        /// Gets the vertexes of the model.
        /// </summary>
        /// <value>
        /// The vertexes of the model.
        /// </value>
        public ModelVertex[] Vertexes { get; private set; }

        /// <summary>
        /// Renders the model using the specified transform and GPU mapping.
        /// </summary>
        /// <param name="renderContext">
        ///     The render context.
        /// </param>
        /// <param name="transform">
        ///     The transform.
        /// </param>
        /// <param name="effectParameterSet"></param>
        /// <param name="effect"></param>
        public void Render(IRenderContext renderContext, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform)
        {
            var request = CreateRenderRequest(renderContext, effect, effectParameterSet, transform);
            _renderBatcher.RenderRequestImmediate(renderContext, request);
        }
        
        /// <summary>
        /// Load the vertex and index buffer for this model.
        /// </summary>
        /// <param name="graphicsDevice">
        /// The graphics device.
        /// </param>
        public void LoadBuffers(GraphicsDevice graphicsDevice)
        {
            if (_indexBuffer == null)
            {
                _indexBuffer = new IndexBuffer(
                    graphicsDevice, 
                    IndexElementSize.ThirtyTwoBits, 
                    Indices.Length, 
                    BufferUsage.WriteOnly);
                _indexBuffer.SetData(Indices);
            }
        }

        /// <summary>
        /// Creates a render request for the model using the specified transform.
        /// </summary>
        /// <param name="renderContext">
        ///     The render context.
        /// </param>
        /// <param name="effect"></param>
        /// <param name="effectParameterSet"></param>
        /// <param name="transform">
        ///     The transform.
        /// </param>
        public IRenderRequest CreateRenderRequest(IRenderContext renderContext, IEffect effect, IEffectParameterSet effectParameterSet, Matrix transform)
        {
            if (Vertexes.Length == 0 && Indices.Length == 0)
            {
                throw new InvalidOperationException(
                    "This model does not have any vertexes or indices.  It's most " +
                    "likely been imported from an FBX file that only contains hierarchy, " +
                    "in which case there isn't anything to render.");
            }

            LoadBuffers(renderContext.GraphicsDevice);

            VertexBuffer vertexBuffer;
            if (_cachedVertexBuffers.ContainsKey(effect))
            {
                vertexBuffer = _cachedVertexBuffers[effect];
            }
            else
            {
                // Find the vertex mapping configuration for this model.
                if (_cachedModelVertexMapping == null)
                {
                    _cachedModelVertexMapping =
                        _modelRenderConfigurations.Select(x => x.GetVertexMappingToGPU(this, effect))
                            .FirstOrDefault(x => x != null);
                    if (_cachedModelVertexMapping == null)
                    {
                        throw new InvalidOperationException(
                            "No implementation of IModelRenderConfiguration could provide a vertex " +
                            "mapping for this model.  You must implement IModelRenderConfiguration " +
                            "and bind it in the dependency injection system, so that the engine is " +
                            "aware of how to map vertices in models to parameters in effects.");
                    }
                }

                var mappedVerticies = Array.CreateInstance(_cachedModelVertexMapping.VertexType, Vertexes.Length);
                for (var i = 0; i < Vertexes.Length; i++)
                {
                    var vertex = _cachedModelVertexMapping.MappingFunction(Vertexes[i]);
                    mappedVerticies.SetValue(vertex, i);
                }

                vertexBuffer = new VertexBuffer(
                    renderContext.GraphicsDevice,
                    _cachedModelVertexMapping.VertexDeclaration,
                    Vertexes.Length,
                    BufferUsage.WriteOnly);
                vertexBuffer.GetType().GetMethods().First(x => x.Name == "SetData" && x.GetParameters().Length == 1).MakeGenericMethod(_cachedModelVertexMapping.VertexType).Invoke(
                    vertexBuffer,
                    new[] { mappedVerticies });
                _cachedVertexBuffers[effect] = vertexBuffer;
            }
            
            if (effectParameterSet.HasSemantic<IBonesEffectSemantic>())
            {
                var bonesEffectSemantic = effectParameterSet.GetSemantic<IBonesEffectSemantic>();

                foreach (var bone in _flattenedBones)
                {
                    if (bone.ID == -1)
                    {
                        continue;
                    }

                    bonesEffectSemantic.Bones[bone.ID] = bone.GetFinalMatrix();
                }
            }

            // Create the render request.
            return _renderBatcher.CreateSingleRequestFromState(
                renderContext,
                effect,
                effectParameterSet,
                vertexBuffer,
                IndexBuffer,
                PrimitiveType.TriangleList,
                renderContext.World*transform, (m, vb, ib) =>
                {
                    var mappedVerticies = Array.CreateInstance(_cachedModelVertexMapping.VertexType, Vertexes.Length * m.Count);
                    var mappedIndicies = new int[Indices.Length*m.Count];

                    for (var im = 0; im < m.Count; im++)
                    {
                        for (var i = 0; i < Vertexes.Length; i++)
                        {
                            var vertex = _cachedModelVertexMapping.MappingFunction(Vertexes[i].Transform(m[im]));
                            mappedVerticies.SetValue(vertex, im*Vertexes.Length+i);
                        }

                        for (var i = 0; i < Indices.Length; i++)
                        {
                            mappedIndicies[im*Vertexes.Length + i] = Indices[i] + Vertexes.Length*im;
                        }
                    }

                    vb.GetType().GetMethods().First(x => x.Name == "SetData" && x.GetParameters().Length == 1).MakeGenericMethod(_cachedModelVertexMapping.VertexType).Invoke(
                        vb,
                        new[] { mappedVerticies });
                    ib.SetData(mappedIndicies);
                });
        }

        public void Dispose()
        {
            if (_indexBuffer != null)
            {
                _indexBuffer.Dispose();
                _indexBuffer = null;
            }
        }
    }
}