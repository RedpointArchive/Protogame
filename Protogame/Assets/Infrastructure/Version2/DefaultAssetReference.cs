using System;
using System.Threading.Tasks;

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
            if (asset == null)
            {
                if (assetState == AssetReferenceState.Ready ||
                    assetState == AssetReferenceState.PartiallyReady)
                {
                    throw new ArgumentNullException("asset", "Asset can not be null when moving reference into Ready or PartiallyReady state.");
                }
            }

            if (_asset != asset && assetState == AssetReferenceState.Ready)
            {
                (_asset as IDisposable)?.Dispose();
            }

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
            if (_asset == null)
            {
                if (assetState == AssetReferenceState.Ready ||
                    assetState == AssetReferenceState.PartiallyReady)
                {
                    throw new ArgumentNullException("asset", "Asset can not be null when moving reference into Ready or PartiallyReady state.");
                }
            }

            State = assetState;
        }

        public async Task WaitUntilReady()
        {
            while (State != AssetReferenceState.Ready &&
                State != AssetReferenceState.Unavailable)
            {
                await Task.Yield();
            }

            if (State == AssetReferenceState.Unavailable)
            {
                throw new AggregateException(LoadingException);
            }
        }
    }
}