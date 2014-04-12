namespace Protogame
{
    using System.Linq;

    /// <summary>
    /// The texture asset saver.
    /// </summary>
    public class TextureAssetSaver : IAssetSaver
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
            return asset is TextureAsset;
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
        public IRawAsset Handle(IAsset asset, AssetTarget target)
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

            return
                new AnonymousObjectBasedRawAsset(
                    new
                    {
                        Loader = typeof(TextureAssetLoader).FullName,
                        PlatformData = target == AssetTarget.SourceFile ? null : textureAsset.PlatformData,
                        RawData =
                            textureAsset.RawData == null ? null : textureAsset.RawData.Select(x => (int)x).ToList()
                    });
        }
    }
}