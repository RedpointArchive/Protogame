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

            if (modelAsset.SourcedFromRaw)
            {
                // We were sourced from a raw FBX; we don't want to save
                // an ".asset" file back to disk.
                return null;
            }
            
            return new
            {
                Loader = typeof(ModelAssetLoader).FullName,
                CompiledData = modelAsset.CompiledData == null || target == AssetTarget.SourceFile ? null : modelAsset.CompiledData.Select(x => (int)x).ToList(),
                SourceData = modelAsset.SourceData == null || target == AssetTarget.CompiledFile ? null : modelAsset.SourceData.Select(x => (int)x).ToList()
            };
        }
    }
}

