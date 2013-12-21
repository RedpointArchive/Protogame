using System;

namespace Protogame
{
    public class NullAssetContentManager : IAssetContentManager
    {
        public void SetStream(string assetName, System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Purge(string assetName)
        {
            throw new NotImplementedException();
        }

        public T Load<T>(string assetName)
        {
            throw new NotImplementedException();
        }

        public void UnsetStream(string assetName)
        {
            throw new NotImplementedException();
        }
    }
}

