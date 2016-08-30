namespace Protogame
{
    public class AudioAssetLoader : IAssetLoader
    {
        private IAssetContentManager m_AssetContentManager;
        
        public AudioAssetLoader(IAssetContentManager assetContentManager)
        {
            this.m_AssetContentManager = assetContentManager;
        }
        
        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(AudioAssetLoader).FullName;
        }

        public IAsset Load(string name, IRawAsset data)
        {
            if (data.IsCompiled)
            {
                return new AudioAsset(name, null, data.GetProperty<PlatformData>("PlatformData"), false);
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

            var audio = new AudioAsset(
                name, 
                ByteReader.ReadAsByteArray(data.GetProperty<object>("RawData")), 
                platformData, 
                data.GetProperty<bool>("SourcedFromRaw"));

            return audio;
        }
    }
}