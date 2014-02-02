namespace Protogame
{
    /// <summary>
    /// The effect asset saver.
    /// </summary>
    public class EffectAssetSaver : IAssetSaver
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
            return asset is EffectAsset;
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
            var effectAsset = asset as EffectAsset;

            if (effectAsset.SourcedFromRaw && target != AssetTarget.CompiledFile)
            {
                // We were sourced from a raw FX; we don't want to save
                // an ".asset" file back to disk.
                return null;
            }

            if (target == AssetTarget.CompiledFile)
            {
                return new CompiledAsset
                {
                    Loader = typeof(EffectAssetLoader).FullName, 
                    PlatformData = effectAsset.PlatformData
                };
            }

            return
                new
                {
                    Loader = typeof(EffectAssetLoader).FullName, 
                    effectAsset.Code, 
                    PlatformData = target == AssetTarget.SourceFile ? null : effectAsset.PlatformData
                };
        }
    }
}