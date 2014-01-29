namespace Protogame
{
    public class LanguageAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is LanguageAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var textAsset = asset as LanguageAsset;

            return new
            {
                Loader = typeof(LanguageAssetLoader).FullName,
                Value = textAsset.Value
            };
        }
    }
}

