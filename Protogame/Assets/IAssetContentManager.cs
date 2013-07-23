using System.IO;

namespace Protogame
{
    public interface IAssetContentManager
    {
        void SetStream(string assetName, Stream stream);
        void Purge(string assetName);
        T Load<T>(string assetName);
        void UnsetStream(string assetName);
    }
}

