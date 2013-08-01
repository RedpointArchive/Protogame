namespace Protogame
{
    public interface ITileUtilities
    {
        /// <summary>
        /// Renders a tile entity to the screen.
        /// </summary>
        /// <param name="entity">The tile entity to render.</param>
        /// <param name="context">The rendering context.</param>
        void RenderTile(ITileEntity entity, IRenderContext renderContext);
        
        /// <summary>
        /// Adjust the tile bounds: X, Y, Width and Height.
        /// </summary>
        /// <param name="entity">The tile entity to render.</param>
        /// <param name="tilesetCellWidth">The tileset cell width.</param>
        /// <param name="tilesetCellHeight">The tileset cell height.</param>
        void AdjustTileBounds(ITileEntity entity, int tilesetCellWidth, int tilesetCellHeight);
    }
}

