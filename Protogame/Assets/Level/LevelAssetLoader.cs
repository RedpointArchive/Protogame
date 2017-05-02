using System.Threading.Tasks;

namespace Protogame
{
    public class LevelAssetLoader : IAssetLoader<LevelAsset>
    {
        public async Task<IAsset> Load(string name, IReadableSerializedAsset input, IAssetManager assetManager)
        {
            return new LevelAsset(
                name,
                input.GetString("LevelData"),
                input.GetString("LevelDataFormat"));
        }
    }
}