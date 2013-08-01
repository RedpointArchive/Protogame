namespace Protogame
{
    public interface ITileEntity : IEntity
    {
        int TX { get; set; }
        int TY { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        TilesetAsset Tileset { get; set; }
    }
}
