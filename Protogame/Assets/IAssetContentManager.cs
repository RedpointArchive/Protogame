using System.IO;

namespace Protogame
{
    public interface IAssetContentManager
    {
        void SetStream(string assetName, Stream stream);
        T Load<T>(string assetName);
        void UnsetStream(string assetName);
    }
}

