namespace Protogame
{
    public interface ITileEntity : IEntity, IHasSize
    {
        int TX { get; set; }
        
        int TY { get; set; }

        bool AppliedTilesetSettings { get; set; }
        
        IAssetReference<TilesetAsset> Tileset { get; set; }
    }
}