namespace Protogame
{
    public class FontAssetLoader : IAssetLoader
    {
        private readonly IAssetContentManager m_AssetContentManager;
        
        public FontAssetLoader(IAssetContentManager assetContentManager)
        {
            this.m_AssetContentManager = assetContentManager;
        }
        
        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(FontAssetLoader).FullName;
        }

        public IAsset Load(string name, IRawAsset data)
        {
            if (data.IsCompiled)
            {
                return new FontAsset(this.m_AssetContentManager, name, null, 0, false, 0, data.GetProperty<PlatformData>("PlatformData"));
            }

            PlatformData platformData = null;
            if (data.GetProperty<PlatformData>("PlatformData") != null)
            {
                platformData = new PlatformData
                {
                    Platform = data.GetProperty<PlatformData>("PlatformData").Platform,
                    Data = ByteReader.ReadAsByteArray(data.GetProperty<PlatformData>("PlatformData").Data)
                };
            }

            var effect = new FontAsset(
                this.m_AssetContentManager, 
                name, 
                data.GetProperty<string>("FontName"), 
                data.GetProperty<int>("FontSize"), 
                data.GetProperty<bool>("UseKerning"), 
                data.GetProperty<int>("Spacing"), 
                platformData);

            return effect;
        }
    }
}