using System.Threading.Tasks;

namespace Protogame
{
    public class TilesetAssetLoader : IAssetLoader<TilesetAsset>
    {
        public async Task<IAsset> Load(string name, IReadableSerializedAsset input, IAssetManager assetManager)
        {
            var textureName = input.GetString("TextureName");

            return new TilesetAsset(
                name,
                textureName,
                assetManager.Get<TextureAsset>(textureName),
                input.GetInt32("CellWidth"),
                input.GetInt32("CellHeight"));
        }
    }
}