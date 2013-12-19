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

            if (target == AssetTarget.CompiledFile)
            {
                return new CompiledAsset
                {
                    Loader = typeof(FontAssetLoader).FullName,
                    PlatformData = fontAsset.PlatformData
                };
            }

            return new
            {
                Loader = typeof(FontAssetLoader).FullName,
                FontSize = fontAsset.FontSize,
                FontName = fontAsset.FontName,
                UseKerning = fontAsset.UseKerning,
                Spacing = fontAsset.Spacing,
                PlatformData = target == AssetTarget.SourceFile ? null : fontAsset.PlatformData
            };
        }
    }
}

