namespace Protogame
{
    public class FontAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is FontAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var fontAsset = asset as FontAsset;
            
            return new
            {
                Loader = typeof(FontAssetLoader).FullName,
                FontSize = target == AssetTarget.CompiledFile ? 0 : fontAsset.FontSize,
                FontName = target == AssetTarget.CompiledFile ? null : fontAsset.FontName,
                UseKerning = target == AssetTarget.CompiledFile ? false : fontAsset.UseKerning,
                Spacing = target == AssetTarget.CompiledFile ? 0 : fontAsset.Spacing,
                PlatformData = target == AssetTarget.SourceFile ? null : fontAsset.PlatformData
            };
        }
    }
}

