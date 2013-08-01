namespace Protogame
{
    public interface ITileEntity : IEntity, IHasSize
    {
        int TX { get; set; }
        int TY { get; set; }
        TilesetAsset Tileset { get; set; }
    }
}
