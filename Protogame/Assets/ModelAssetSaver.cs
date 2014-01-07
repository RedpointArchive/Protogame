using System.Linq;

namespace Protogame
{
    public class ModelAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is ModelAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var modelAsset = asset as ModelAsset;

            if (modelAsset.SourcedFromRaw && target != AssetTarget.CompiledFile)
            {
                // We were sourced from a raw FBX; we don't want to save
                // an ".asset" file back to disk.
                return null;
            }

            if (target == AssetTarget.CompiledFile)
            {
                return new CompiledAsset
                {
                    Loader = typeof(ModelAssetLoader).FullName,
                    PlatformData = modelAsset.PlatformData
                };
            }
            
            return new
            {
                Loader = typeof(ModelAssetLoader).FullName,
                PlatformData = target == AssetTarget.SourceFile ? null : modelAsset.PlatformData,
                RawData = modelAsset.RawData == null ? null : modelAsset.RawData.Select(x => (int)x).ToList()
            };
        }
    }
}

