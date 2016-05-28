namespace Protogame
{
    /// <summary>
    /// The model asset loader.
    /// </summary>
    public class ModelAssetLoader : IAssetLoader
    {
        private readonly IModelSerializer _modelSerializer;

        /// <summary>
        /// The m_ asset content manager.
        /// </summary>
        private IAssetContentManager m_AssetContentManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAssetLoader"/> class.
        /// </summary>
        /// <param name="modelSerializer">
        /// The model serializer service.
        /// </param>
        /// <param name="assetContentManager">
        /// The asset content manager.
        /// </param>
        public ModelAssetLoader(IModelSerializer modelSerializer, IAssetContentManager assetContentManager)
        {
            _modelSerializer = modelSerializer;
            this.m_AssetContentManager = assetContentManager;
        }

        /// <summary>
        /// The can handle.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanHandle(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(ModelAssetLoader).FullName;
        }

        /// <summary>
        /// The can new.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanNew()
        {
            return true;
        }

        /// <summary>
        /// The get default.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return null;
        }

        /// <summary>
        /// The get new.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new ModelAsset(_modelSerializer, name, null, null, null, false, string.Empty);
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        public IAsset Handle(IAssetManager assetManager, string name, IRawAsset data)
        {
            if (data is CompiledAsset)
            {
                return new ModelAsset(_modelSerializer, name, null, null, data.GetProperty<PlatformData>("PlatformData"), false, string.Empty);
            }

            PlatformData platformData = null;
            if (data.GetProperty<PlatformData>("PlatformData") != null)
            {
                platformData = new PlatformData
                {
                    Platform = data.GetProperty<PlatformData>("PlatformData").Platform,
                    Data = data.GetProperty<PlatformData>("PlatformData").Data
                };
            }

            var model = new ModelAsset(
                _modelSerializer,
                name, 
                ByteReader.ReadAsByteArray(data.GetProperty<object>("RawData")),
                data.GetProperty<System.Collections.Generic.Dictionary<string, byte[]>>("RawAdditionalAnimations"), 
                platformData,
                data.GetProperty<bool>("SourcedFromRaw"),
                data.GetProperty<string>("Extension"));

            return model;
        }
    }
}