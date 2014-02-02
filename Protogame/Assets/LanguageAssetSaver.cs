namespace Protogame
{
    /// <summary>
    /// The language asset saver.
    /// </summary>
    public class LanguageAssetSaver : IAssetSaver
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
            return asset is LanguageAsset;
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
            var textAsset = asset as LanguageAsset;

            return new { Loader = typeof(LanguageAssetLoader).FullName, textAsset.Value };
        }
    }
}