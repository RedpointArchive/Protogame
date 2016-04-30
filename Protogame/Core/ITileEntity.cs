namespace Protogame
{
    /// <summary>
    /// The TileEntity interface.
    /// </summary>
    public interface ITileEntity : IEntity, IHasSize
    {
        /// <summary>
        /// Gets or sets the tx.
        /// </summary>
        /// <value>
        /// The tx.
        /// </value>
        int TX { get; set; }

        /// <summary>
        /// Gets or sets the ty.
        /// </summary>
        /// <value>
        /// The ty.
        /// </value>
        int TY { get; set; }

        /// <summary>
        /// Gets or sets the tileset.
        /// </summary>
        /// <value>
        /// The tileset.
        /// </value>
        TilesetAsset Tileset { get; set; }
    }
}