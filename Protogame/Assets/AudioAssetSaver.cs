namespace Protogame
{
    public class AudioAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is AudioAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            var audioAsset = asset as AudioAsset;
            
            return new
            {
                Loader = typeof(AudioAssetLoader).FullName,
                AudioData = audioAsset.Data,
                SourcePath = audioAsset.SourcePath
            };
        }
    }
}

