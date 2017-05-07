using System.Threading.Tasks;

namespace Protogame
{
    public class UserInterfaceAssetLoader : IAssetLoader<UserInterfaceAsset>
    {
        public async Task<IAsset> Load(string name, IReadableSerializedAsset input, IAssetManager assetManager)
        {
            return new UserInterfaceAsset(
                name,
                input.GetString("UserInterfaceData"),
                input.GetString("UserInterfaceFormat"));
        }
    }
}