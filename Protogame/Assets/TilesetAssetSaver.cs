namespace Protogame
{
    public class TilesetAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is TilesetAsset;
        }

        public dynamic Handle(IAsset asset)
        {
            var tilesetAsset = asset as TilesetAsset;
            
            return new
            {
                Loader = typeof(TilesetAssetLoader).FullName,
                TextureName = tilesetAsset.Texture != null ? tilesetAsset.Texture.Name : null,
                CellWidth = tilesetAsset.CellWidth,
                CellHeight = tilesetAsset.CellHeight
            };
        }
    }
}

