using System;

namespace Protogame
{
    public class TilesetAssetLoader : IAssetLoader
    {
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(TilesetAssetLoader).FullName;
        }

        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return new TilesetAsset(
                assetManager,
                name,
                data.TextureName,
                data.CellWidth,
                data.CellHeight);
        }

        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return null;
        }
        
        public bool CanNew()
        {
            return true;
        }
        
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new TilesetAsset(
                assetManager,
                name,
                "",
                16,
                16);
        }
    }
}

