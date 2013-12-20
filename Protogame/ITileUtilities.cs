namespace Protogame
{
    /// <summary>
    /// The TileUtilities interface.
    /// </summary>
    public interface ITileUtilities
    {
        /// <summary>
        /// Initializes the tile using the information in the tileset asset.
        /// </summary>
        /// <param name="entity">
        /// The tile entity to initialize.
        /// </param>
        /// <param name="tilesetAssetName">
        /// The tileset asset name.
        /// </param>
        void InitializeTile(ITileEntity entity, string tilesetAssetName);

        /// <summary>
        /// Renders a tile entity to the screen.
        /// </summary>
        /// <param name="entity">
        /// The tile entity to render.
        /// </param>
        /// <param name="renderContext">
        /// The rendering context to use.
        /// </param>
        void RenderTile(ITileEntity entity, IRenderContext renderContext);
    }
}