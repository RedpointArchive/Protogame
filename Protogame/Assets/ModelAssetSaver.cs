namespace Protogame
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The model asset saver.
    /// </summary>
    public class ModelAssetSaver : IAssetSaver
    {
        /// <summary>
        /// The can handle.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanHandle(IAsset asset)
        {
            return asset is ModelAsset;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="dynamic"/>.
        /// </returns>
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

            return
                new
                {
                    Loader = typeof(ModelAssetLoader).FullName, 
                    PlatformData = target == AssetTarget.SourceFile ? null : modelAsset.PlatformData, 
                    RawData = modelAsset.RawData == null ? null : modelAsset.RawData.Select(x => (int)x).ToList(), 
                    RawAdditionalAnimations = (Dictionary<string, byte[]>)null
                };
        }
    }
}