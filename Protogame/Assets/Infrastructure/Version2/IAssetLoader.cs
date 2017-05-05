using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetLoader
    {
        Task<IAsset> Load(string name, IReadableSerializedAsset input, IAssetManager assetManager);
    }

    public interface IAssetLoader<T> : IAssetLoader
    {   
    }
}
