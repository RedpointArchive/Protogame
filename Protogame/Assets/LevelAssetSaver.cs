//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//

namespace Protogame
{
    public class LevelAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is LevelAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var levelAsset = asset as LevelAsset;

            return new
            {
                Loader = typeof(LevelAssetLoader).FullName,
                Value = levelAsset.Value,
                SourcePath = levelAsset.SourcePath
            };
        }
    }
}

