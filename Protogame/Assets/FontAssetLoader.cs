namespace Protogame
{
    /// <summary>
    /// The font asset loader.
    /// </summary>
    public class FontAssetLoader : IAssetLoader
    {
        /// <summary>
        /// The m_ asset content manager.
        /// </summary>
        private readonly IAssetContentManager m_AssetContentManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontAssetLoader"/> class.
        /// </summary>
        /// <param name="assetContentManager">
        /// The asset content manager.
        /// </param>
        public FontAssetLoader(IAssetContentManager assetContentManager)
        {
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
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(FontAssetLoader).FullName;
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
            return new FontAsset(this.m_AssetContentManager, name, string.Empty, 0, true, 0, null);
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
        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            if (data is CompiledAsset)
            {
                return new FontAsset(this.m_AssetContentManager, name, null, 0, false, 0, data.PlatformData);
            }

            PlatformData platformData = null;
            if (data.PlatformData != null)
            {
                platformData = new PlatformData
                {
                    Platform = data.PlatformData.Platform, 
                    Data = ByteReader.ReadAsByteArray(data.PlatformData.Data)
                };
            }

            var effect = new FontAsset(
                this.m_AssetContentManager, 
                name, 
                (string)data.FontName, 
                (int)data.FontSize, 
                (bool)data.UseKerning, 
                (int)data.Spacing, 
                platformData);

            return effect;
        }
    }
}