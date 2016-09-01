namespace Protogame
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a model asset.
    /// </summary>
    public class ModelAsset : MarshalByRefObject, IAsset
    {
        private readonly IModelSerializer _modelSerializer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAsset"/> class.
        /// <para>
        /// This constructor is called by <see cref="ModelAssetLoader"/> when loading a model asset.
        /// </para>
        /// </summary>
        /// <param name="modelSerializer">
        ///     The model serializer service.
        /// </param>
        /// <param name="name">
        ///     The name of the model asset.
        /// </param>
        /// <param name="rawData">
        ///     The raw, serialized model data.
        /// </param>
        /// <param name="rawAdditionalAnimations">
        ///     The source raw data that contains additional animations, mapping byte arrays to animation names.
        /// </param>
        /// <param name="data">
        ///     The platform specific data.
        /// </param>
        /// <param name="sourcedFromRaw">
        ///     Whether or not this asset was sourced from a purely raw asset file (such as a PNG).
        /// </param>
        /// <param name="extension">
        ///     The appropriate file extension for this model.
        /// </param>
        /// <param name="importOptions"></param>
        public ModelAsset(IModelSerializer modelSerializer, string name, byte[] rawData, Dictionary<string, byte[]> rawAdditionalAnimations, PlatformData data, bool sourcedFromRaw, string extension, string[] importOptions)
        {
            _modelSerializer = modelSerializer;
            Name = name;
            RawData = rawData;
            RawAdditionalAnimations = rawAdditionalAnimations;
            PlatformData = data;
            SourcedFromRaw = sourcedFromRaw;
            Extension = extension;
            ImportOptions = importOptions;
        }

        /// <summary>
        /// Gets a value indicating whether the asset only contains compiled information.
        /// </summary>
        /// <value>
        /// Whether the asset only contains compiled information.
        /// </value>
        public bool CompiledOnly => RawData == null;

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
        public bool SourceOnly => PlatformData == null;

        /// <summary>
        /// Gets a value indicating whether or not this asset was sourced from a raw file (such as a FBX file).
        /// </summary>
        /// <value>
        /// The sourced from raw.
        /// </value>
        public bool SourcedFromRaw { get; private set; }

        /// <summary>
        /// Gets the additional options used for the initial import.  If this is null, then no options file was
        /// present, otherwise it is an explicit list of options that were set during the import.
        /// <para>
        /// This value is always null at runtime.
        /// </para>
        /// </summary>
        public string[] ImportOptions { get; set; }

        /// <summary>
        /// Instantiates a model object from the model asset.  Each model object has it's own vertex and index
        /// buffers, so you should aim to share model objects which will always have the same state.
        /// </summary>
        /// <returns>The model object.</returns>
        public IModel InstantiateModel()
        {
            if (PlatformData != null)
            {
                return _modelSerializer.Deserialize(Name, PlatformData.Data);
            }

            throw new InvalidOperationException("Attempted to instantiate a model, but no platform data was loaded.");
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