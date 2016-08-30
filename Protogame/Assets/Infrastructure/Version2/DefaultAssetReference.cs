using System;

namespace Protogame
{
    public class DefaultAssetReference<T> : ISingleAssetReference<T> where T : class, IAsset
    {
        private T _asset;

        public DefaultAssetReference(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public T Asset
        {
            get
            {
                switch (State)
                {
                    case AssetReferenceState.NotReady:
                        return null;
                    case AssetReferenceState.PartiallyReady:
                    case AssetReferenceState.Ready:
                        return _asset;
                    case AssetReferenceState.Unavailable:
                        throw new AggregateException(LoadingException);
                }

                throw new InvalidOperationException("Unexpected state for asset reference.");
            }
        }

        public AssetReferenceState State { get; set; }

        public bool IsReady => State == AssetReferenceState.Ready;

        public Exception LoadingException { get; set; }

        public void Update(IAsset asset, AssetReferenceState assetState)
        {
            _asset = (T) asset;
            State = assetState;
        }

        public void Update(Exception exception)
        {
            LoadingException = exception;
            State = AssetReferenceState.Unavailable;
        }

        public void Update(AssetReferenceState assetState)
        {
            State = assetState;
        }
    }
}