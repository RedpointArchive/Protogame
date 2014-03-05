namespace Protogame
{
    public class TextureAtlasAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is TextureAtlasAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var textureAtlasAsset = (TextureAtlasAsset)asset;

            if (target == AssetTarget.CompiledFile)
            {
                return new CompiledAsset
                {
                    Loader = typeof(TextureAtlasAssetLoader).FullName, 
                    PlatformData = new PlatformData
                    {
                        Platform = textureAtlasAsset.AtlasTexture.PlatformData.Platform,
                        Data = textureAtlasAsset.GetCompiledData()
                    }
                };
            }

            // TODO: Are there any scenarios where we want to save the .asset file?
            return null;
        }
    }
}

