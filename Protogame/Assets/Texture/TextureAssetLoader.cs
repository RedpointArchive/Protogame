namespace Protogame
{
    public class TextureAssetLoader : IAssetLoader
    {
        private readonly IAssetContentManager m_AssetContentManager;
        
        public TextureAssetLoader(IAssetContentManager assetContentManager)
        {
            this.m_AssetContentManager = assetContentManager;
        }

        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(TextureAssetLoader).FullName;
        }
        
        public IAsset Load(string name, IRawAsset data)
        {
            if (data is CompiledAsset)
            {
                return new TextureAsset(this.m_AssetContentManager, name, null, data.GetProperty<PlatformData>("PlatformData"), false);
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

            var texture = new TextureAsset(
                this.m_AssetContentManager,
                name,
                ByteReader.ReadAsByteArray(data.GetProperty<object>("RawData")),
                platformData,
                data.GetProperty<bool>("SourcedFromRaw"));

            return texture;
        }
    }
}