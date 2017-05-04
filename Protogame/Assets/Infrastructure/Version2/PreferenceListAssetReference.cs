using System;
using System.Linq;
using System.Threading.Tasks;

namespace Protogame
{
    public class PreferenceListAssetReference<T> : IAssetReference<T> where T : class, IAsset
    {
        private readonly IAssetReference<T>[] _preferenceList;

        public PreferenceListAssetReference(IAssetReference<T>[] preferenceList)
        {
            _preferenceList = preferenceList;
        }

        public T Asset
        {
            get
            {
                for (var i = 0; i < _preferenceList.Length; i++)
                {
                    var assetRef = _preferenceList[i];
                    if (assetRef.State == AssetReferenceState.Unavailable)
                    {
                        // Try next.
                        continue;
                    }

                    if (assetRef.State == AssetReferenceState.Ready)
                    {
                        return assetRef.Asset;
                    }

                    return null;
                }

                throw LoadingException;
            }
        }

        public AssetReferenceState State
        {
            get
            {
                for (var i = 0; i < _preferenceList.Length; i++)
                {
                    var assetRef = _preferenceList[i];
                    if (assetRef.State == AssetReferenceState.Unavailable)
                    {
                        // Try next.
                        continue;
                    }

                    if (assetRef.State == AssetReferenceState.Ready)
                    {
                        return AssetReferenceState.Ready;
                    }

                    return AssetReferenceState.NotReady;
                }

                return AssetReferenceState.Unavailable;
            }
        }

        public bool IsReady => State == AssetReferenceState.Ready;

        public Exception LoadingException
        {
            get
            {
                return new AggregateException(_preferenceList.Where(x => x.LoadingException != null).Select(x => x.LoadingException));
            }
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
                throw LoadingException;
            }

            // Otherwise, asset is ready.
        }
    }
}
