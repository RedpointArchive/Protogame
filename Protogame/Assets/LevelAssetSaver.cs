namespace Protogame
{
    /// <summary>
    /// The level asset saver.
    /// </summary>
    public class LevelAssetSaver : IAssetSaver
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
            return asset is LevelAsset;
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
            var levelAsset = asset as LevelAsset;

            return new { Loader = typeof(LevelAssetLoader).FullName, levelAsset.Value, levelAsset.SourcePath };
        }
    }
}