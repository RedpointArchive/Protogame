namespace Protogame
{
    public class AudioAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is AudioAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var audioAsset = asset as AudioAsset;
            
            return new
            {
                Loader = typeof(AudioAssetLoader).FullName,
                PlatformData = audioAsset.PlatformData,
                SourcePath = audioAsset.SourcePath
            };
        }
    }
}

