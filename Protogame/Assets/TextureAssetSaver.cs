namespace Protogame
{
    public class TextureAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is TextureAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            var textureAsset = asset as TextureAsset;
            
            return new
            {
                Loader = typeof(TextureAssetLoader).FullName,
                TextureData = textureAsset.Data,
                SourcePath = textureAsset.SourcePath
            };
        }
    }
}

