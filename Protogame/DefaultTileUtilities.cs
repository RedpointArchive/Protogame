namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The default tile utilities.
    /// </summary>
    public class DefaultTileUtilities : ITileUtilities
    {
        /// <summary>
        /// The m_ asset manager.
        /// </summary>
        private readonly IAssetManager m_AssetManager;

        /// <summary>
        /// The m_ render utilities.
        /// </summary>
        private readonly I2DRenderUtilities m_RenderUtilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTileUtilities"/> class.
        /// </summary>
        /// <param name="renderUtilities">
        /// The render utilities.
        /// </param>
        /// <param name="assetManagerProvider">
        /// The asset manager provider.
        /// </param>
        public DefaultTileUtilities(I2DRenderUtilities renderUtilities, IAssetManagerProvider assetManagerProvider)
        {
            this.m_RenderUtilities = renderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
        }

        /// <summary>
        /// The initialize tile.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="tilesetAssetName">
        /// The tileset asset name.
        /// </param>
        public void InitializeTile(ITileEntity entity, string tilesetAssetName)
        {
            entity.Tileset = this.m_AssetManager.Get<TilesetAsset>(tilesetAssetName);
            entity.X *= entity.Tileset.CellWidth;
            entity.Y *= entity.Tileset.CellHeight;
            entity.Width = entity.Tileset.CellWidth;
            entity.Height = entity.Tileset.CellHeight;
        }

        /// <summary>
        /// The render tile.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        public void RenderTile(ITileEntity entity, IRenderContext renderContext)
        {
            this.m_RenderUtilities.RenderTexture(
                renderContext, 
                new Vector2(entity.X, entity.Y), 
                entity.Tileset.Texture, 
                new Vector2(entity.Width, entity.Height), 
                sourceArea:
                    new Rectangle(
                        entity.TX * entity.Tileset.CellWidth, 
                        entity.TY * entity.Tileset.CellHeight, 
                        entity.Tileset.CellWidth, 
                        entity.Tileset.CellHeight));
        }
    }
}