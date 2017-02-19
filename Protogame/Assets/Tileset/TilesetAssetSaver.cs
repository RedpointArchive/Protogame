namespace Protogame
{
    /// <summary>
    /// The tileset asset saver.
    /// </summary>
    public class TilesetAssetSaver : IAssetSaver
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
            return asset is TilesetAsset;
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
            var tilesetAsset = asset as TilesetAsset;

            return
                new AnonymousObjectBasedRawAsset(
                    new
                    {
                        Loader = typeof(TilesetAssetLoader).FullName,
                        TextureName = tilesetAsset?.Texture?.Asset?.Name,
                        tilesetAsset.CellWidth,
                        tilesetAsset.CellHeight
                    });
        }
    }
}