namespace Protogame
{
    public class ModelAssetLoader : IAssetLoader
    {
        private IAssetContentManager m_AssetContentManager;

        public ModelAssetLoader(
            IAssetContentManager assetContentManager)
        {
            this.m_AssetContentManager = assetContentManager;
        }
    
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(ModelAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            if (data is CompiledAsset)
            {
                return new ModelAsset(
                    name,
                    null,
                    data.CompiledData,
                    data.SourcedFromRaw != null && (bool)data.SourcedFromRaw);
            }

            var texture = new ModelAsset(
                name,
                ByteReader.ReadAsByteArray(data.SourceData),
                ByteReader.ReadAsByteArray(data.CompiledData),
                data.SourcedFromRaw != null && (bool)data.SourcedFromRaw);

            return texture;
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return null;
        }
        
        public bool CanNew()
        {
            return true;
        }
        
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new ModelAsset(
                name,
                null,
                null,
                false);
        }
    }
}

