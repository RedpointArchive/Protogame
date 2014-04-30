namespace Protogame
{
    public class VariableAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is VariableAsset;
        }

        public IRawAsset Handle(IAsset asset, AssetTarget target)
        {
            var variableAsset = asset as VariableAsset;

            return
                new AnonymousObjectBasedRawAsset(
                    new { Loader = typeof(LevelAssetLoader).FullName, Value = variableAsset.RawValue });
        }
    }
}