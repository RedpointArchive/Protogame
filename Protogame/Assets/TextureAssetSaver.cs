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

            if (textureAsset.SourcedFromRaw && target != AssetTarget.CompiledFile)
            {
                // We were sourced from a raw PNG; we don't want to save
                // an ".asset" file back to disk.
                return null;
            }

            if (target == AssetTarget.CompiledFile)
            {
                return new CompiledAsset
                {
                    Loader = typeof(TextureAssetLoader).FullName,
                    PlatformData = textureAsset.PlatformData
                };
            }
            
            return new
            {
                Loader = typeof(TextureAssetLoader).FullName,
                PlatformData = target == AssetTarget.SourceFile ? null : textureAsset.PlatformData,
                RawData = textureAsset.RawData == null ? null : textureAsset.RawData.Select(x => (int)x).ToList()
            };
        }
    }
}

