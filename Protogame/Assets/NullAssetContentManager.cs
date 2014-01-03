using System;

namespace Protogame
{
    public class NullAssetContentManager : IAssetContentManager
    {
        public void SetStream(string assetName, System.IO.Stream stream)
        {
            throw new NoAssetContentManagerException();
        }

        public void Purge(string assetName)
        {
            throw new NoAssetContentManagerException();
        }

        public T Load<T>(string assetName)
        {
            throw new NoAssetContentManagerException();
        }

        public void UnsetStream(string assetName)
        {
            throw new NoAssetContentManagerException();
        }
    }
}

