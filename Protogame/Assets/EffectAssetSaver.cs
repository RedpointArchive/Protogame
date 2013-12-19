namespace Protogame
{
    public class EffectAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is EffectAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var effectAsset = asset as EffectAsset;

            if (target == AssetTarget.CompiledFile)
            {
                return new CompiledAsset
                {
                    Loader = typeof(EffectAssetLoader).FullName,
                    PlatformData = effectAsset.PlatformData
                };
            }
            
            return new
            {
                Loader = typeof(EffectAssetLoader).FullName,
                SourcePath = effectAsset.SourcePath,
                PlatformData = effectAsset.PlatformData
            };
        }
    }
}

