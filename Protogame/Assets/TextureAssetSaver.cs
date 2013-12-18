using System.Linq;

namespace Protogame
{
    public class TextureAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is TextureAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var textureAsset = asset as TextureAsset;
            
            return new
            {
                Loader = typeof(TextureAssetLoader).FullName,
                PlatformData = target == AssetTarget.SourceFile ? null : textureAsset.PlatformData,
                RawData = target == AssetTarget.CompiledFile ? null : (textureAsset.RawData == null ? null : textureAsset.RawData.Select(x => (int)x).ToList())
            };
        }
    }
}

